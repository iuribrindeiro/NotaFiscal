CREATE DATABASE NotaFiscalDb;
GO

USE NotaFiscalDb;

CREATE TABLE TipoRps (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Discriminator NVARCHAR(50) NOT NULL,
    Descricao NVARCHAR(MAX) NOT NULL
);

CREATE INDEX TipoRps_Discriminator_Index ON TipoRps (Discriminator);


CREATE TABLE RegimeEspecialTributacao (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Discriminator NVARCHAR(50) NOT NULL,
    Descricao NVARCHAR(MAX) NOT NULL
);

CREATE INDEX RegimeEspecialTributacao_Discriminator_Index ON RegimeEspecialTributacao (Discriminator);

CREATE TABLE NaturezaOperacao (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Discriminator NVARCHAR(50) NOT NULL,
    Descricao NVARCHAR(MAX) NOT NULL
);

CREATE INDEX NaturezaOperacao_Discriminator_Index ON NaturezaOperacao (Discriminator);


CREATE TABLE Endereco (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Rua NVARCHAR(120) NOT NULL,
    Numero NVARCHAR(60) NOT NULL,
    Complemento NVARCHAR(60) NULL,
    Bairro NVARCHAR(60) NOT NULL,
    CodigoMunicipio NVARCHAR(7) NOT NULL,
    Cep NVARCHAR(8) NOT NULL,
    Uf CHAR(2) NOT NULL,
)

CREATE TABLE Contato (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Telefone NVARCHAR(20) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
)


CREATE TABLE Tomador (
    Id INT PRIMARY KEY,
    CpfCnpj NVARCHAR(20) NOT NULL,
    InscricaoMunicipal NVARCHAR(15) NULL,
    RazaoSocial NVARCHAR(100) NULL,
    EnderecoId INT FOREIGN KEY REFERENCES Endereco(Id) NULL,
    ContatoId INT FOREIGN KEY REFERENCES Contato(Id) NULL,
)

CREATE TABLE NotaFiscalServico (
    Id INT PRIMARY KEY,
    Discriminator NVARCHAR(50) NOT NULL,
    DataCriacao DATETIME NOT NULL DEFAULT(GETUTCDATE()),
    DataAlteracao DATETIME NULL,
    PrestadorCnpj NVARCHAR(20) NOT NULL,
    PrestadorInscricaoMunicipal NVARCHAR(15) NOT NULL,
    TomadorDiscriminator NVARCHAR(50) NOT NULL,
    TomadorId INT FOREIGN KEY REFERENCES Tomador(Id) NULL,
    ServicoRegimeEspecialTributacaoId INT FOREIGN KEY REFERENCES RegimeEspecialTributacao(Id) NOT NULL,
    ServicoNaturezaOperacaoId INT FOREIGN KEY REFERENCES NaturezaOperacao(Id) NOT NULL,
    TipoRpsId INT FOREIGN KEY REFERENCES TipoRps(Id) NULL,
    RpsNumero INT NULL,
    RpsSerie NVARCHAR(5) NULL,
    DataEmissao DATETIME NULL,
    NumeroNota int NULL,
    NumeroLote NVARCHAR(20) NULL,
    CodigoCancelamento NVARCHAR(50) NULL,
    ValoresServicos DECIMAL(18,2) NOT NULL,
    ValoresDeducoes DECIMAL(18,2),
    ValoresPis DECIMAL(18,2),
    ValoresCofins DECIMAL(18,2),
    ValoresInss DECIMAL(18,2),
    ValoresIr DECIMAL(18,2),
    ValoresCsll DECIMAL(18,2),
    ValoresIss DECIMAL(18,2) NOT NULL,
    ValoresIssDiscriminator NVARCHAR(50) NOT NULL,
    ValoresOutrasRetencoes DECIMAL(18,2),
    ValoresDescontoCondicionado DECIMAL(18,2),
    ValoresDescontoIncondicionado DECIMAL(18,2),
    ValoresAliquota FLOAT,
    ItemListaServico NVARCHAR(255) NOT NULL,
    CodigoTributacaoMunicipio NVARCHAR(20) NULL,
    Discriminacao NVARCHAR(MAX) NOT NULL,
    MunicipioPrestacaoServico NVARCHAR(7) NOT NULL,
    CodigoCnae NVARCHAR(7) NULL,
    OptanteSimplesNacional BIT NOT NULL,
    IncentivadorCultural BIT NOT NULL
);

GO

CREATE TABLE ErroComunicacao (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    NotaFiscalServicoId INT FOREIGN KEY REFERENCES NotaFiscalServico(Id) NOT NULL,
    CodigoErro NVARCHAR(100) NOT NULL,
    Mensagem NVARCHAR(100) NOT NULL,
    Correcao NVARCHAR(MAX) NULL,
);

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
