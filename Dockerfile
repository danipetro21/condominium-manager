FROM mcr.microsoft.com/dotnet/sdk:8.0

# Installa le dipendenze necessarie
RUN apt-get update && apt-get install -y \
    curl \
    git \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY . .
RUN dotnet restore

# Genera il certificato HTTPS
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=SecurePassword123!
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/root/.aspnet/https/aspnetapp.pfx
RUN mkdir -p /root/.aspnet/https && \
    dotnet dev-certs https -ep /root/.aspnet/https/aspnetapp.pfx -p SecurePassword123!

EXPOSE 5000 5001

ENTRYPOINT ["dotnet", "watch", "run", "--urls", "http://0.0.0.0:5000;https://0.0.0.0:5001"] 