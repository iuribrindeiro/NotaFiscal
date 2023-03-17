-- Fix all compilation errors in this script

CREATE DATABASE NotaFiscalDb;
GO

USE NotaFiscalDb;

CREATE TABLE TipoRps (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Discriminator VARCHAR(50) NOT NULL,
    Descricao VARCHAR(MAX) NOT NULL
);

CREATE INDEX TipoRps_Discriminator_Index ON TipoRps (Discriminator);


CREATE TABLE MensagemRetorno (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Discriminator VARCHAR(50) NOT NULL,
    TipoRpsId INT FOREIGN KEY REFERENCES TipoRps(Id) NOT NULL,
    RpsNumero INT NOT NULL,
    RpsSerie INT NOT NULL,
    CodigoMensagemAlerta VARCHAR(100) NOT NULL,
    Mensagem VARCHAR(100) NOT NULL,
    Correcao VARCHAR(MAX) NULL,
);

CREATE INDEX MensagemRetorno_Discriminator_Index ON MensagemRetorno (Discriminator);

CREATE TABLE RegimeEspecialTributacao (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Discriminator VARCHAR(50) NOT NULL,
    Descricao VARCHAR(MAX) NOT NULL
);

CREATE INDEX RegimeEspecialTributacao_Discriminator_Index ON RegimeEspecialTributacao (Discriminator);

CREATE TABLE NaturezaOperacao (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Discriminator VARCHAR(50) NOT NULL,
    Descricao VARCHAR(MAX) NOT NULL
);

CREATE INDEX NaturezaOperacao_Discriminator_Index ON NaturezaOperacao (Discriminator);

CREATE TABLE NotaFiscalServico (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Discriminator VARCHAR(50) NOT NULL,
    DataCriacao DATETIME NOT NULL DEFAULT(GETUTCDATE()),
    DataAlteracao DATETIME NULL,
    PrestadorCnpj VARCHAR(20) NOT NULL,
    PrestadorInscricaoMunicipal int NULL,
    TomadorDiscriminator VARCHAR(50) NOT NULL,
    TomadorCpfCnpj VARCHAR(20) NULL,
    TomadorInscricaoMunicipal int NULL,
    TomadorRazaoSocial VARCHAR(100) NULL,
    TomadorEnderecoEndereco VARCHAR(100) NULL,
    TomadorEnderecoNumero VARCHAR(20) NULL,
    TomadorEnderecoComplemento VARCHAR(100) NULL,
    TomadorEnderecoBairro VARCHAR(50) NULL,
    TomadorEnderecoCidade VARCHAR(50) NULL,
    TomadorEnderecoEstado VARCHAR(2) NULL,
    TomadorEnderecoCep VARCHAR(10) NULL,
    TomadorContatoTelefone VARCHAR(20) NULL,
    TomadorContatoEmail VARCHAR(100) NULL,
    ServicoRegimeEspecialTributacaoId INT FOREIGN KEY REFERENCES RegimeEspecialTributacao(Id) NOT NULL,
    ServicoNaturezaOperacaoId INT FOREIGN KEY REFERENCES NaturezaOperacao(Id) NOT NULL,
    TipoRpsId INT FOREIGN KEY REFERENCES TipoRps(Id),
    RpsNumero INT NOT NULL,
    RpsSerie INT NOT NULL,
    DataEmissao DATETIME NULL,
    NumeroNota VARCHAR(20) NULL,
    NumeroLote VARCHAR(20) NULL,
    CodigoCancelamento VARCHAR(50) NULL,
    CodigoMensagemAlerta VARCHAR(50) NULL,
    Mensagem VARCHAR(500) NULL,
    Correcao VARCHAR(500) NULL,
    ValoresServicos DECIMAL(18,2) NOT NULL,
    ValoresDeducoes DECIMAL(18,2),
    ValoresPis DECIMAL(18,2),
    ValoresCofins DECIMAL(18,2),
    ValoresInss DECIMAL(18,2),
    ValoresIr DECIMAL(18,2),
    ValoresCsll DECIMAL(18,2),
    ValorLiquidoNfse DECIMAL(18,2),
    ValoresIss DECIMAL(18,2) NOT NULL,
    ValoresIssDiscriminator VARCHAR(50) NOT NULL,
    ValoresOutrasRetencoes DECIMAL(18,2),
    ValoresBaseCalculo DECIMAL(18,2),
    ValoresDescontoCondicionado DECIMAL(18,2),
    ValoresDescontoIncondicionado DECIMAL(18,2),
    ValoresAliquota DECIMAL(5,2),
    ItemListaServico NVARCHAR(255) NOT NULL,
    CodigoTributacaoMunicipio NVARCHAR(255) NOT NULL,
    Discriminacao NVARCHAR(MAX) NOT NULL,
    MunicipioPrestacaoServico NVARCHAR(255) NOT NULL,
    OptanteSimplesNacional BIT NOT NULL,
    IncentivadorCultural BIT NOT NULL
);

GO

CREATE INDEX NotaFiscalServico_Discriminator_Index ON NotaFiscalServico (Discriminator);
CREATE INDEX NotaFiscalServico_Valores_Iss_Discriminator_Index ON NotaFiscalServico (ValoresIssDiscriminator);
CREATE INDEX NotaFiscalServico_Tomador_Discriminator_Index ON NotaFiscalServico (TomadorDiscriminator);

GO

CREATE TRIGGER DataAlteracao_NotaFiscalServico_Update
ON NotaFiscalServico
AFTER UPDATE
AS
BEGIN
    UPDATE NotaFiscalServico
    SET DataAlteracao = GETUTCDATE()
    FROM NotaFiscalServico
    INNER JOIN inserted
    ON NotaFiscalServico.Id = inserted.Id;
END;

CREATE TABLE MensagemRetornoNotaFiscalServico
(
    NotaFiscalServicoId UNIQUEIDENTIFIER,
    MensagemRetornoId INT,
    CONSTRAINT PK_NotaFiscalServicoMensagemRetono PRIMARY KEY (NotaFiscalServicoId, MensagemRetornoId),
    CONSTRAINT FK_NotaFiscalServicoMensagemRetorno_NotaId FOREIGN KEY (NotaFiscalServicoId) REFERENCES NotaFiscalServico(Id),
    CONSTRAINT FK_NotaFiscalServicoMensagemRetorno_MensagemId FOREIGN KEY (MensagemRetornoId) REFERENCES MensagemRetorno(Id)
)

GO


INSERT INTO TipoRps
( -- Columns to insert data into
 [Discriminator], [Descricao]
)
VALUES
( -- First row: values for the columns in the list above
 'Rps', 'Rps'
),
( -- Second row: values for the columns in the list above
 'NotaFiscalConjugadaMista', 'Nota Fiscal Conjugada (Mista)'
),
( -- Second row: values for the columns in the list above
 'Cupom', 'Cupom'
)



INSERT INTO RegimeEspecialTributacao
( -- Columns to insert data into
 [Discriminator], [Descricao]
)
VALUES
( -- First row: values for the columns in the list above
 'MicroempresaMunicipal', 'Microempresa Municipal'
),
( -- Second row: values for the columns in the list above
 'Estimativa', 'Estimativa'
),
( -- Second row: values for the columns in the list above
 'SociedadeProfissionais', 'Sociedade de Profissionais'
),
( -- Second row: values for the columns in the list above
 'Cooperativa', 'Cooperativa'
),
( -- Second row: values for the columns in the list above
 'MicroempreendedorIndividual', 'Microempresário Individual (MEI)'
),
( -- Second row: values for the columns in the list above
 'MicroempreendedorPequenoPorte', 'Microempresário e Empresa de Pequeno Porte (ME EPP)'
)



-- Insert rows into table 'TableName' in schema '[dbo]'
INSERT INTO NaturezaOperacao
( -- Columns to insert data into
 [Discriminator], [Descricao]
)
VALUES
( -- First row: values for the columns in the list above
 'TributacaoMunicipio', 'Tributação no municipio'
),
( -- Second row: values for the columns in the list above
 'TributacaoForaMunicipio', 'Tributação fora do municipio'
),
( -- Second row: values for the columns in the list above
 'Isencao', 'Isenção'
),
( -- Second row: values for the columns in the list above
 'Imune', 'Imune'
),
( -- Second row: values for the columns in the list above
 'DecisaoJudicial', 'Exigibilidade suspensa por decisão judicial'
),
( -- Second row: values for the columns in the list above
 'ProcedimentoAdministrativo', 'Exigibilidade suspensa por procedimento administrativo'
)
