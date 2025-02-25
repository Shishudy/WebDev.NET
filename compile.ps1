# Diretório de saída
$OUTPUT_DIR = 'DLLs/gdb'
$DLL_DIR = 'DLLs'
if (!(Test-Path -Path $OUTPUT_DIR)) {
    New-Item -ItemType Directory -Path $OUTPUT_DIR
}
if (!(Test-Path -Path $DLL_DIR)) {
    New-Item -ItemType Directory -Path $DLL_DIR
}

Write-Host '🔧 (USER) Compilando RegrasDeAcesso...'
dotnet build ./3_SPA/LibEF/LibEF.csproj -c Release -o $OUTPUT_DIR

Write-Host '🔧 Compilando WebAPI...'
dotnet build ./1_WebApi/WebAPI.csproj -c Release -o $OUTPUT_DIR

# Write-Host '🔧 (USER) Compilando RegrasDeNegocio...'
# dotnet build SolucaoPrincipal/ProjetoRegrasDeNegocio/RegrasDeNegocio.csproj -c Release -o $OUTPUT_DIR

# Write-Host '🔧 (ADMIN) Compilando RegrasDeAcesso...'
# dotnet build ../LibADO/LibADO/LibADO.csproj -c Release -o $OUTPUT_DIR

# Write-Host '🔧 (ADMIN) Compilando RegrasDeNegocio...'
# ./4_MPA/UserMPA/UserMPA/UserMPA.csproj

# Write-Host '🔧 (ADMIN) Compilando Tester...'
# ./TesteApp/TesteApp/TesteApp.csproj

# Copy only DLL files to the DLL directory
Get-ChildItem -Path $OUTPUT_DIR -Filter *.dll | ForEach-Object {
    Copy-Item -Path $_.FullName -Destination $DLL_DIR
}

# Remove-Item -Recurse -Force $OUTPUT_DIR

Write-Host '✅ Build concluído! DLLs em $OUTPUT_DIR'