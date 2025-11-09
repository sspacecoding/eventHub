# Script PowerShell para Limpar e Reiniciar Docker
# Execute este script para tentar consertar problemas do Docker

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "  SpaceEventHub - Limpeza do Docker" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# Verificar se está executando como Administrador
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

if (-not $isAdmin) {
    Write-Host "⚠️  AVISO: Este script funciona melhor quando executado como Administrador" -ForegroundColor Yellow
    Write-Host "   Clique com botão direito no PowerShell e selecione 'Executar como Administrador'" -ForegroundColor Yellow
    Write-Host ""
    $continue = Read-Host "Deseja continuar mesmo assim? (S/N)"
    if ($continue -ne "S" -and $continue -ne "s") {
        exit
    }
    Write-Host ""
}

# Passo 1: Parar containers
Write-Host "📦 Passo 1: Parando containers..." -ForegroundColor Yellow
try {
    docker-compose down 2>$null
    Write-Host "✅ Containers parados" -ForegroundColor Green
} catch {
    Write-Host "⚠️  Nenhum container em execução" -ForegroundColor Yellow
}
Write-Host ""

# Passo 2: Limpar cache de build
Write-Host "🧹 Passo 2: Limpando cache de build..." -ForegroundColor Yellow
try {
    $output = docker builder prune -a -f 2>&1
    Write-Host "✅ Cache limpo" -ForegroundColor Green
    Write-Host "   $output" -ForegroundColor Gray
} catch {
    Write-Host "❌ Erro ao limpar cache: $_" -ForegroundColor Red
}
Write-Host ""

# Passo 3: Limpar volumes
Write-Host "💾 Passo 3: Limpando volumes não utilizados..." -ForegroundColor Yellow
try {
    $output = docker volume prune -f 2>&1
    Write-Host "✅ Volumes limpos" -ForegroundColor Green
    Write-Host "   $output" -ForegroundColor Gray
} catch {
    Write-Host "❌ Erro ao limpar volumes: $_" -ForegroundColor Red
}
Write-Host ""

# Passo 4: Limpar imagens
Write-Host "🖼️  Passo 4: Limpando imagens não utilizadas..." -ForegroundColor Yellow
try {
    $output = docker image prune -a -f 2>&1
    Write-Host "✅ Imagens limpas" -ForegroundColor Green
    Write-Host "   $output" -ForegroundColor Gray
} catch {
    Write-Host "❌ Erro ao limpar imagens: $_" -ForegroundColor Red
}
Write-Host ""

# Passo 5: Verificar espaço em disco
Write-Host "💿 Passo 5: Verificando espaço em disco..." -ForegroundColor Yellow
try {
    $driveC = Get-PSDrive C
    $freeSpaceGB = [math]::Round($driveC.Free / 1GB, 2)
    $usedSpaceGB = [math]::Round($driveC.Used / 1GB, 2)
    $totalSpaceGB = [math]::Round(($driveC.Free + $driveC.Used) / 1GB, 2)
    
    Write-Host "   Drive C:" -ForegroundColor Cyan
    Write-Host "   - Total: $totalSpaceGB GB" -ForegroundColor Gray
    Write-Host "   - Usado: $usedSpaceGB GB" -ForegroundColor Gray
    Write-Host "   - Livre: $freeSpaceGB GB" -ForegroundColor Gray
    
    if ($freeSpaceGB -lt 10) {
        Write-Host "⚠️  AVISO: Pouco espaço livre! Recomendado: 20GB+" -ForegroundColor Yellow
    } else {
        Write-Host "✅ Espaço em disco OK" -ForegroundColor Green
    }
} catch {
    Write-Host "⚠️  Não foi possível verificar espaço em disco" -ForegroundColor Yellow
}
Write-Host ""

# Passo 6: Verificar processos do Docker
Write-Host "🔍 Passo 6: Verificando processos do Docker..." -ForegroundColor Yellow
$dockerProcesses = Get-Process | Where-Object { $_.Name -like "*docker*" -or $_.Name -like "*com.docker*" }
if ($dockerProcesses) {
    Write-Host "   Processos Docker encontrados:" -ForegroundColor Cyan
    foreach ($proc in $dockerProcesses) {
        Write-Host "   - $($proc.Name) (PID: $($proc.Id))" -ForegroundColor Gray
    }
    Write-Host "✅ Docker está em execução" -ForegroundColor Green
} else {
    Write-Host "⚠️  Nenhum processo Docker encontrado" -ForegroundColor Yellow
    Write-Host "   Certifique-se de que o Docker Desktop está aberto" -ForegroundColor Yellow
}
Write-Host ""

# Passo 7: Informações do Docker
Write-Host "ℹ️  Passo 7: Informações do Docker..." -ForegroundColor Yellow
try {
    $dockerVersion = docker --version
    $composeVersion = docker-compose --version
    Write-Host "   Docker: $dockerVersion" -ForegroundColor Gray
    Write-Host "   Docker Compose: $composeVersion" -ForegroundColor Gray
    Write-Host "✅ Docker está instalado" -ForegroundColor Green
} catch {
    Write-Host "❌ Docker não está instalado ou não está no PATH" -ForegroundColor Red
}
Write-Host ""

# Resumo
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "           RESUMO DA LIMPEZA" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "✅ Limpeza concluída!" -ForegroundColor Green
Write-Host ""
Write-Host "📋 Próximos passos:" -ForegroundColor Yellow
Write-Host ""
Write-Host "1. Feche o Docker Desktop completamente:" -ForegroundColor White
Write-Host "   - Clique com botão direito no ícone do Docker" -ForegroundColor Gray
Write-Host "   - Selecione 'Quit Docker Desktop'" -ForegroundColor Gray
Write-Host ""
Write-Host "2. Aguarde 10 segundos" -ForegroundColor White
Write-Host ""
Write-Host "3. Abra o Docker Desktop novamente" -ForegroundColor White
Write-Host ""
Write-Host "4. Aguarde até o ícone ficar verde" -ForegroundColor White
Write-Host ""
Write-Host "5. Execute o comando:" -ForegroundColor White
Write-Host "   docker-compose up -d --build" -ForegroundColor Cyan
Write-Host ""
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# Perguntar se deseja fechar o Docker Desktop automaticamente
Write-Host "💡 Dica: Este script pode tentar fechar o Docker Desktop automaticamente" -ForegroundColor Yellow
$closeDocker = Read-Host "Deseja fechar o Docker Desktop agora? (S/N)"

if ($closeDocker -eq "S" -or $closeDocker -eq "s") {
    Write-Host ""
    Write-Host "🔄 Fechando Docker Desktop..." -ForegroundColor Yellow
    
    # Tentar fechar o Docker Desktop
    try {
        # Parar o serviço do Docker (requer Admin)
        if ($isAdmin) {
            Stop-Service -Name "com.docker.service" -Force -ErrorAction SilentlyContinue
            Write-Host "✅ Serviço Docker parado" -ForegroundColor Green
        }
        
        # Fechar processos do Docker Desktop
        Get-Process | Where-Object { $_.Name -like "*Docker Desktop*" } | Stop-Process -Force -ErrorAction SilentlyContinue
        Get-Process | Where-Object { $_.Name -eq "Docker Desktop" } | Stop-Process -Force -ErrorAction SilentlyContinue
        
        Write-Host "✅ Docker Desktop fechado" -ForegroundColor Green
        Write-Host ""
        Write-Host "⏳ Aguardando 10 segundos..." -ForegroundColor Yellow
        Start-Sleep -Seconds 10
        Write-Host ""
        Write-Host "✅ Agora você pode abrir o Docker Desktop novamente" -ForegroundColor Green
        Write-Host ""
        
        $openDocker = Read-Host "Deseja abrir o Docker Desktop agora? (S/N)"
        if ($openDocker -eq "S" -or $openDocker -eq "s") {
            Write-Host "🚀 Abrindo Docker Desktop..." -ForegroundColor Yellow
            Start-Process "C:\Program Files\Docker\Docker\Docker Desktop.exe" -ErrorAction SilentlyContinue
            Write-Host "✅ Docker Desktop iniciado" -ForegroundColor Green
            Write-Host "⏳ Aguarde até o ícone ficar verde antes de continuar" -ForegroundColor Yellow
        }
    } catch {
        Write-Host "⚠️  Não foi possível fechar o Docker Desktop automaticamente" -ForegroundColor Yellow
        Write-Host "   Feche manualmente: botão direito no ícone → Quit Docker Desktop" -ForegroundColor Gray
    }
}

Write-Host ""
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "Script concluído!" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Se o problema persistir, consulte:" -ForegroundColor Yellow
Write-Host "CONSERTAR_DOCKER_PASSO_A_PASSO.md" -ForegroundColor Cyan
Write-Host ""
