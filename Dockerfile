FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

RUN apt-get update && apt-get install -y python3 python3-pip pipenv

COPY ./MovieWatch.Backend/Pipfile ./MovieWatch.Backend/Pipfile.lock /

WORKDIR /

ENV PIPENV_VENV_IN_PROJECT=1

RUN pipenv install

WORKDIR /App


ENTRYPOINT ["dotnet", "MovieWatch.Api.dll"]
