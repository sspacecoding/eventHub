# Script PowerShell para inserir dados de exemplo no banco de dados
# SpaceEventHub - Seed Data Script

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Inserindo dados de exemplo no banco" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$sqlScript = @"
USE SpaceEventHubDB;
GO

-- Limpar dados existentes (opcional)
DELETE FROM PageViews;
DELETE FROM Notifications;
DELETE FROM EventRegistrations;
DELETE FROM Events;
DELETE FROM Users;
GO

-- Inserir Usuários
-- Nota: As senhas são 'Admin@123', 'Organizer@123', 'Attendee@123'
-- Os hashes BCrypt serão gerados pelo backend quando fizer login pela primeira vez
-- Por enquanto usamos placeholders que serão substituídos quando o backend fizer hash
INSERT INTO Users (Email, PasswordHash, FirstName, LastName, Role, IsActive, CreatedAt)
VALUES 
('admin@spaceeventhub.com', '`$2a`$11`$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5qT5R5Q6q5Q6q', 'Admin', 'User', 'Admin', 1, GETDATE()),
('organizer@spaceeventhub.com', '`$2a`$11`$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5qT5R5Q6q5Q6q', 'John', 'Organizer', 'Organizer', 1, GETDATE()),
('attendee@spaceeventhub.com', '`$2a`$11`$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5qT5R5Q6q5Q6q', 'Jane', 'Attendee', 'Attendee', 1, GETDATE()),
('maria.silva@email.com', '`$2a`$11`$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5qT5R5Q6q5Q6q', 'Maria', 'Silva', 'Attendee', 1, GETDATE()),
('pedro.costa@email.com', '`$2a`$11`$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5qT5R5Q6q5Q6q', 'Pedro', 'Costa', 'Organizer', 1, GETDATE()),
('ana.santos@email.com', '`$2a`$11`$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5qT5R5Q6q5Q6q', 'Ana', 'Santos', 'Attendee', 1, GETDATE());
GO
"@

Write-Host "Inserindo usuários..." -ForegroundColor Yellow
docker exec -i spaceeventhub-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong@Passw0rd -d SpaceEventHubDB -Q $sqlScript

Write-Host "Dados inseridos com sucesso!" -ForegroundColor Green

