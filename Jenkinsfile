pipeline {
    agent any

    environment {
        projectName = 'WebApi'
        publishedPath = 'src/WebApi/bin/Release/net7.0/publish/.'
        serviceName = 'app' // Вставить наименование дериктории для развертывания приложения.
        servicesPath = '/services/'
    }

    stages {
        stage('Build Release') {
            steps {
                dir('src/WebApi') {
                    sh 'dotnet build ${projectName}.csproj -c Release'
                    sh 'dotnet publish -c Release --no-build'
                }
            }
        }

        stage('Deploy') {
            when {
                expression {
                    currentBuild.result == null || currentBuild.result == 'SUCCESS'
                }
            }
            steps {
                sh 'sudo mkdir -p ${servicesPath}${serviceName}'
                sh 'sudo cp -r ${publishedPath} ${servicesPath}${serviceName}'
            }
        }

        stage('Create service') {
            steps {
                dir('/etc/systemd/system') { // Для выполнения необходимы права для записи в etc и т.д. .
                    sh '''sudo echo "[Unit]\n" >> ''' + serviceName + '''.service
                    echo "Description=web api of ${servicesName}\n\n" >> ''' + serviceName + '''.service
                    echo "[Service]\n" >> ''' + serviceName + '''.service
                    echo "ExecStart=${servicesPath}${serviceName}/${projectName}\n" >> ''' + serviceName + '''.service
                    echo "WorkingDirectory=${servicesPath}${serviceName}\n" >> ''' + serviceName + '''.service
                    echo "Restart=always\n" >> ''' + serviceName + '''.service
                    echo "RestartSec=5\n\n" >> ''' + serviceName + '''.service
                    echo "[Install]\n" >> ''' + serviceName + '''.service
                    echo "WantedBy=multi-user.target" >> ''' + serviceName + '''.service
                    '''
                }
            }
        }

        stage('Restart service') {
            steps {
                sh 'sudo service ${serviceName} restart'
            }
        }
    }
}
