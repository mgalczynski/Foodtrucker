pipeline {
    environment {
        registry = 'docker.miroslawgalczynski.com'
        registryCredential = 'nexus'
        tag = "${(GIT_BRANCH == 'origin/master') ? 'latest' : GIT_BRANCH - 'origin/'}"
        image = null
    }
    agent any

    stages {
        stage('Build and publish base image') {
            steps {
                script {
                    docker.withRegistry('https://' + registry, registryCredential) {
                        image = docker.build(registry + '/dotnet:2.2-sdk-node', '-f Dockerfile-dotnet-sdk-node .')
                        image.push()
                    }
                }
            }
        }
        stage('Tests') {
            steps {
                script {
                    docker.image('mdillon/postgis:11').withRun('-e "POSTGRES_PASSWORD=postgres" -e "POSTGRES_DB=FoodtruckerTest"') { c ->
                        docker.image('mdillon/postgis:11').inside("--link ${c.id}:db") {
                            sh 'for ty in {1..10} ; do [ -n $(pg_isready -d FoodtruckerTest -h db -U postgres -q) ] && break; sleep 1; done'
                        }
                        docker.withRegistry('https://' + registry, registryCredential) {
                            docker.image('docker.miroslawgalczynski.com/dotnet:2.2-sdk-node').inside("--link ${c.id}:db -e \"DOTNET_CLI_TELEMETRY_OPTOUT=1\"") {
                                sh 'dotnet test'
                            }
                        }
                    }
                }
            }
        }
        stage('Building and publish image') {
            steps {
                script {
                    docker.withRegistry('https://' + registry, registryCredential) {
                        image = docker.build registry + '/foodtrucker:' + tag
                        image.push()
                    }
                }
            }
        }
        stage('Deploying image') {
            agent {
                docker { image 'kroniak/ssh-client' }
            }
            when { expression { tag == 'latest' } }
            steps {
                withCredentials([sshUserPrivateKey(credentialsId: 'ssh', keyFileVariable: 'keyFile', usernameVariable: 'userName'),
                                 usernamePassword(credentialsId: 'nexus', usernameVariable: 'dockerUserName', passwordVariable: 'dockerPassword')]) {
                    script {
                        sh 'mkdir -p ~/.ssh'
                        sh 'cat known_hosts > ~/.ssh/known_hosts'
                        sh 'ssh -p 45 -i ${keyFile} ${userName}@foodtrucker.miroslawgalczynski.com docker login --username ${dockerUserName} --password ${dockerPassword} ${registry}'
                        sh 'scp -P 45 -i ${keyFile} update.sh ${userName}@foodtrucker.miroslawgalczynski.com:'
                        sh 'scp -P 45 -i ${keyFile} development.yml ${userName}@foodtrucker.miroslawgalczynski.com:'
                        sh 'ssh -p 45 -i ${keyFile} ${userName}@foodtrucker.miroslawgalczynski.com PORT=8080 sh update.sh'
                        sh 'ssh -p 45 -i ${keyFile} ${userName}@foodtrucker.miroslawgalczynski.com rm update.sh'
                        sh 'ssh -p 45 -i ${keyFile} ${userName}@foodtrucker.miroslawgalczynski.com rm development.yml'
                        sh 'ssh -p 45 -i ${keyFile} ${userName}@foodtrucker.miroslawgalczynski.com docker logout ${registry}'
                    }
                }
            }
        }
    }
}

