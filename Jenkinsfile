pipeline {
    environment {
        registry = 'docker.miroslawgalczynski.com'
        registryCredential = 'nexus'
        image = null
    }
    agent any

    stages {
        stage('Building image') {
            steps{
                script {
                     image = docker.build registry + '/foodtrucker'
                }
            }
        }
        stage('Publishing image') {
            steps{
                script {
                    docker.withRegistry('https://' + registry, registryCredential) {
                        image.push()
                    }
                }
            }
        }
        stage('Deploying image') {
            agent {
                docker { image 'kroniak/ssh-client' }
            }
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
