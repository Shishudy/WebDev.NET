#!/bin/bash


## this is an idea to auto compile all sln files into output dir.
## i can maybe search all csproj files and try compile them into an project specific folder.
set -e  # Para parar em caso de erro

# Diretório de saída
OUTPUT_DIR="Output"
mkdir -p $OUTPUT_DIR

echo "🔧 Compilando ProjetoBD..."
dotnet build SolucaoPrincipal/ProjetoBD/BD.csproj -c Release -o $OUTPUT_DIR

echo "🔧 Compilando RegrasDeNegocio..."
dotnet build SolucaoPrincipal/ProjetoRegrasDeNegocio/RegrasDeNegocio.csproj -c Release -o $OUTPUT_DIR

echo "🔧 Compilando WebAPI..."
dotnet build SolucaoPrincipal/ProjetoWebAPI/WebAPI.csproj -c Release -o $OUTPUT_DIR

echo "✅ Build concluído! DLLs em $OUTPUT_DIR"

