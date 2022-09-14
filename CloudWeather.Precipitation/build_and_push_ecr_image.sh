set -e
aws ecr get-login-password --region eu-west-2 --profile weather-ecr-agent | docker login --username AWS --password-stdin 426132332230.dkr.ecr.eu-west-2.amazonaws.com
docker build -f ./Dockerfile -t cloud-weather-precipitation:latest .
docker tag cloud-weather-precipitation:latest 426132332230.dkr.ecr.eu-west-2.amazonaws.com/cloud-weather-precipitation:latest
docker push 426132332230.dkr.ecr.eu-west-2.amazonaws.com/cloud-weather-precipitation:latest
