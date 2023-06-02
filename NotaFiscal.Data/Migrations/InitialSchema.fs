module NotaFiscal.Data.Migrations.InitialSchema

open FluentMigrator

[<Migration(20230323210200L)>]
type InitialSchema() =
    inherit Migration()

    override this.Up() =
        this.Create
            .Table("TipoRps")
            .WithColumn("Id")
            .AsGuid()
            .PrimaryKey()
            .WithColumn("Discriminator")
            .AsString(50)
            .NotNullable()
            .WithColumn("Descricao")
            .AsString(100)
            .NotNullable()
        |> ignore

        this.Create
            .Index("TipoRps_Discriminator_Index")
            .OnTable("TipoRps")
            .OnColumn("Discriminator")
            .Ascending()
        |> ignore

        this.Create
            .Table("RegimeEspecialTributacao")
            .WithColumn("Id")
            .AsGuid()
            .PrimaryKey()
            .WithColumn("Discriminator")
            .AsString(50)
            .NotNullable()
            .WithColumn("Descricao")
            .AsString(100)
            .NotNullable()
        |> ignore

        this.Create
            .Index("RegimeEspecialTributacao_Discriminator_Index")
            .OnTable("RegimeEspecialTributacao")
            .OnColumn("Discriminator")
            .Ascending()
        |> ignore


        this.Create
            .Table("NaturezaOperacao")
            .WithColumn("Id")
            .AsGuid()
            .PrimaryKey()
            .WithColumn("Discriminator")
            .AsString(50)
            .NotNullable()
            .WithColumn("Descricao")
            .AsString(100)
            .NotNullable()
        |> ignore

        this.Create
            .Index("NaturezaOperacao_Discriminator_Index")
            .OnTable("NaturezaOperacao")
            .OnColumn("Discriminator")
            .Ascending()
        |> ignore

        this.Create
            .Table("Contato")
            .WithColumn("Id")
            .AsGuid()
            .PrimaryKey()
            .WithColumn("Telefone")
            .AsString(20)
            .NotNullable()
            .WithColumn("Email")
            .AsString(100)
            .NotNullable()
        |> ignore


        this.Create
            .Table("Endereco")
            .WithColumn("Id")
            .AsGuid()
            .PrimaryKey()
            .WithColumn("Rua")
            .AsString(120)
            .NotNullable()
            .WithColumn("Numero")
            .AsString(60)
            .NotNullable()
            .WithColumn("Complemento")
            .AsString(60)
            .Nullable()
            .WithColumn("Bairro")
            .AsString(60)
            .NotNullable()
            .WithColumn("CodigoMunicipio")
            .AsString(7)
            .NotNullable()
            .WithColumn("Cep")
            .AsString(8)
            .NotNullable()
            .WithColumn("Uf")
            .AsString(2)
            .NotNullable()
        |> ignore

        this.Create
            .Table("Tomador")
            .WithColumn("Id")
            .AsGuid()
            .PrimaryKey()
            .WithColumn("CpfCnpj")
            .AsString(20)
            .NotNullable()
            .WithColumn("InscricaoMunicipal")
            .AsString(15)
            .Nullable()
            .WithColumn("RazaoSocial")
            .AsString(100)
            .Nullable()
            .WithColumn("EnderecoId")
            .AsGuid()
            .ForeignKey("FK_Tomador_Endereco", "Endereco", "Id")
            .Nullable()
            .WithColumn("ContatoId")
            .AsGuid()
            .ForeignKey("FK_Tomador_Contato", "Contato", "Id")
            .Nullable()
        |> ignore

        this.Create
            .Table("NotaFiscalServico")
            .WithColumn("Id")
            .AsGuid()
            .PrimaryKey()
            .WithColumn("Discriminator")
            .AsString(50)
            .NotNullable()
            .WithColumn("DataCriacao")
            .AsDateTime()
            .NotNullable()
            .WithDefault(SystemMethods.CurrentUTCDateTime)
            .WithColumn("DataAlteracao")
            .AsDateTime()
            .Nullable()
            .WithColumn("PrestadorCnpj")
            .AsString(20)
            .NotNullable()
            .WithColumn("PrestadorInscricaoMunicipal")
            .AsString(15)
            .NotNullable()
            .WithColumn("TomadorDiscriminator")
            .AsString(50)
            .NotNullable()
            .WithColumn("TomadorId")
            .AsGuid()
            .ForeignKey("FK_NotaFiscalServico_Tomador", "Tomador", "Id")
            .Nullable()
            .WithColumn("ServicoRegimeEspecialTributacaoId")
            .AsInt32()
            .ForeignKey(
                "FK_NotaFiscalServico_RegimeEspecialTributacao",
                "RegimeEspecialTributacao",
                "Id"
            )
            .NotNullable()
            .WithColumn("ServicoNaturezaOperacaoId")
            .AsInt32()
            .ForeignKey(
                "FK_NotaFiscalServico_NaturezaOperacao",
                "NaturezaOperacao",
                "Id"
            )
            .NotNullable()
            .WithColumn("TipoRpsId")
            .AsInt32()
            .ForeignKey("FK_NotaFiscalServico_TipoRps", "TipoRps", "Id")
            .Nullable()
            .WithColumn("RpsNumero")
            .AsInt32()
            .Nullable()
            .WithColumn("RpsSerie")
            .AsString(5)
            .Nullable()
            .WithColumn("DataEmissao")
            .AsDateTime()
            .Nullable()
            .WithColumn("NumeroNota")
            .AsInt32()
            .Nullable()
            .WithColumn("NumeroProtocolo")
            .AsString(50)
            .Nullable()
            .WithColumn("NumeroLote")
            .AsString(20)
            .Nullable()
            .WithColumn("CodigoCancelamento")
            .AsString(50)
            .Nullable()
            .WithColumn("ValoresServicos")
            .AsDecimal(18, 2)
            .NotNullable()
            .WithColumn("ValoresDeducoes")
            .AsDecimal(18, 2)
            .Nullable()
            .WithColumn("ValoresPis")
            .AsDecimal(18, 2)
            .Nullable()
            .WithColumn("ValoresCofins")
            .AsDecimal(18, 2)
            .Nullable()
            .WithColumn("ValoresInss")
            .AsDecimal(18, 2)
            .Nullable()
            .WithColumn("ValoresIr")
            .AsDecimal(18, 2)
            .Nullable()
            .WithColumn("ValoresCsll")
            .AsDecimal(18, 2)
            .Nullable()
            .WithColumn("ValoresIss")
            .AsDecimal(18, 2)
            .NotNullable()
            .WithColumn("ValoresIssDiscriminator")
            .AsString(50)
            .NotNullable()
            .WithColumn("ValoresOutrasRetencoes")
            .AsDecimal(18, 2)
            .Nullable()
            .WithColumn("ValoresDescontoCondicionado")
            .AsDecimal(18, 2)
            .Nullable()
            .WithColumn("ValoresDescontoIncondicionado")
            .AsDecimal(18, 2)
            .Nullable()
            .WithColumn("ValoresAliquota")
            .AsDouble()
            .Nullable()
            .WithColumn("ItemListaServico")
            .AsString(255)
            .NotNullable()
            .WithColumn("CodigoTributacaoMunicipio")
            .AsString(20)
            .Nullable()
            .WithColumn("Discriminacao")
            .AsString(250)
            .NotNullable()
            .WithColumn("MunicipioPrestacaoServico")
            .AsString(7)
            .NotNullable()
            .WithColumn("CodigoCnae")
            .AsString(7)
            .Nullable()
            .WithColumn("OptanteSimplesNacional")
            .AsBoolean()
            .NotNullable()
            .WithColumn("IncentivadorCultural")
            .AsBoolean()
            .NotNullable()
        |> ignore

        this.Execute.Sql(
            "CREATE TRIGGER [NotaFiscalServico_DataAlteracao] ON [NotaFiscalServico] AFTER UPDATE AS BEGIN UPDATE [NotaFiscalServico] SET [DataAlteracao] = GETUTCDATE() WHERE [Id] = (SELECT [Id] FROM INSERTED) END"
        )

        this.Create
            .Index("NotaFiscalServico_Discriminator_Index")
            .OnTable("NotaFiscalServico")
            .OnColumn("Discriminator")
            .Ascending()
        |> ignore

        this.Create
            .Index("NotaFiscalServico_Valores_Iss_Discriminator_Index")
            .OnTable("NotaFiscalServico")
            .OnColumn("ValoresIssDiscriminator")
            .Ascending()
        |> ignore

        this.Create
            .Index("NotaFiscalServico_Tomador_Discriminator_Index")
            .OnTable("NotaFiscalServico")
            .OnColumn("TomadorDiscriminator")
            .Ascending()
        |> ignore


        this.Create
            .Table("ErroComunicacao")
            .WithColumn("Id")
            .AsGuid()
            .PrimaryKey()
            .WithColumn("NotaFiscalServicoId")
            .AsGuid()
            .ForeignKey(
                "FK_ErroComunicacao_NotaFiscalServico",
                "NotaFiscalServico",
                "Id"
            )
            .NotNullable()
            .WithColumn("CodigoErro")
            .AsString(100)
            .NotNullable()
            .WithColumn("Mensagem")
            .AsString(100)
            .NotNullable()
            .WithColumn("Correcao")
            .AsString(100)
            .Nullable()
        |> ignore

        ()

    override this.Down() =
        this.Delete.Table("Contato") |> ignore
        ()