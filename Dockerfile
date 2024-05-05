FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base

WORKDIR /App
COPY . ./
# Install the dependencies
RUN dotnet restore
COPY . ./

FROM base as builder

# Build the release version of Boolean
RUN dotnet publish -c Release -o out

FROM builder as development

WORKDIR /App/Boolean
CMD dotnet run
