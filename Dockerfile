#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat


#This will be the BASE Image for this docker file craeted image
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
#EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

#Copy .csproj file from the host machine TO Core.API.AdditionalService of the folder in the container
COPY ["Core.API.AdditionalService/Core.API.AdditionalService.csproj", "Core.API.AdditionalService/"]
COPY ["Core.API.AdditionalService.Business/Core.API.AdditionalService.Business.csproj", "Core.API.AdditionalService.Business/"]

RUN dotnet restore "Core.API.AdditionalService/Core.API.AdditionalService.csproj"

#Copy current folder in the host machine(.) TO current folder in the container(.) - current folder is src
COPY . .

WORKDIR "/src/Core.API.AdditionalService"
RUN dotnet build "Core.API.AdditionalService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Core.API.AdditionalService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Core.API.AdditionalService.dll"]