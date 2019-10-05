FROM mcr.microsoft.com/dotnet/core/sdk:2.2-alpine AS build
# build image app folder
WORKDIR /app 

# Copy everything else and publish (publish will build and restore as necessary)
COPY . ./

ARG BUILD_CONFIG=Release
RUN echo "Publishing in $BUILD_CONFIG mode"
RUN dotnet publish ./src/ForumSiteCore.Web/ForumSiteCore.Web.csproj -c $BUILD_CONFIG -o out --source https://api.nuget.org/v3/index.json

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-alpine
ARG BUILD_CONFIG=Release
RUN if [ "$BUILD_CONFIG" = "Debug" ]; \
    then apk update && \
    apk add unzip procps curl && \
    curl -sSL https://aka.ms/getvsdbgsh | /bin/sh /dev/stdin -v latest -l /vsdbg; \
    else echo "Building in $BUILD_CONFIG mode, skipping install of remote debugging tools."; \
    fi
EXPOSE 80
EXPOSE 443
WORKDIR /app

ADD https://github.com/ufoscout/docker-compose-wait/releases/download/2.5.1/wait /wait
RUN chmod +x /wait

COPY --from=build /app/src/ForumSiteCore.Web/out .

CMD /wait && dotnet ForumSiteCore.Web.dll