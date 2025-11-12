#!/bin/bash
set -e

echo "Aguardando SQL Server iniciar..."

# Tenta conectar até obter resposta válida
until /opt/mssql-tools/bin/sqlcmd -S database -U sa -P "$MSSQL_SA_PASSWORD" -Q "SELECT 1" -C > /dev/null 2>&1
do
  echo "SQL Server não está pronto ainda..."
  sleep 3
done

echo "SQL Server está pronto!"
echo "Executando script SQL..."

# Executa os comandos do arquivo
/opt/mssql-tools/bin/sqlcmd -S database -U sa -P "$MSSQL_SA_PASSWORD" -C -i /init.sql

echo "Script executado com sucesso!"