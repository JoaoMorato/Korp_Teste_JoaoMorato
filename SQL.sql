IF NOT EXISTS (SELECT 1
FROM sys.databases
WHERE [NAME] = N'Korp')
    CREATE DATABASE Korp;
GO
USE Korp;
GO
IF OBJECT_ID(N'dbo.Produto') IS NULL
BEGIN
    CREATE TABLE [Produto]
    (
        Id VARCHAR(10) NOT NULL PRIMARY KEY,
        Descricao VARCHAR(100) NOT NULL,
        Saldo INT NOT NULL
    );
END
GO
IF OBJECT_ID(N'dbo.NotaFiscal') IS NULL
BEGIN
    CREATE TABLE [NotaFiscal]
    (
        Id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
        Aberta BIT NOT NULL DEFAULT(1),
        DataCriacao DATETIME2(7) NOT NULL DEFAULT(GETDATE()),
        DataFechamento DATETIME2(7)
    );
END
GO
IF OBJECT_ID(N'dbo.ProdutoNotaFiscal') IS NULL
BEGIN
    CREATE TABLE [ProdutoNotaFiscal]
    (
        Id BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
        ProdutoId VARCHAR(10) NOT NULL REFERENCES [Produto](Id),
        NotaFiscalId INT NOT NULL REFERENCES [NotaFiscal](Id),
        Quantidade INT NOT NULL
    );
END