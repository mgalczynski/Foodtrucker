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
    }
}