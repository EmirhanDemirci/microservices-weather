set -e
aws ecr get-login-password --region eu-west-2 --profile weather-ecr-agent | docker login --username AWS --password-stdin 426132332230.dkr.ecr.eu-west-2.amazonaws.com
docker build -f ./Dockerfile -t cloud-weather-temperature:latest .
docker tag cloud-weather-temperature:latest 426132332230.dkr.ecr.eu-west-2.amazonaws.com/cloud-weather-temperature:latest
docker push 426132332230.dkr.ecr.eu-west-2.amazonaws.com/cloud-weather-temperature:latest
