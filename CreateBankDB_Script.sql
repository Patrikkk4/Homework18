CREATE DATABASE TestBankLocalDB

CREATE TABLE [dbo].[Clients] (
    [Id]               INT           IDENTITY (1, 1) NOT NULL,
    [LastName]         NVARCHAR (50) NOT NULL,
    [Name]             NVARCHAR (50) NOT NULL,
    [Patronymic]       NVARCHAR (50) NOT NULL,
    [ClientStatus]     NVARCHAR (50) NOT NULL,
    [RegistrationDate] NVARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
		
CREATE TABLE [dbo].[Bills] (
    [Id]               INT           IDENTITY (1, 1) NOT NULL,
    [ClientID]         INT           NOT NULL,
    [BillNum]          INT           NOT NULL,
    [InclusionDate]    NVARCHAR (50) NOT NULL,
    [InclusionDateEnd] NVARCHAR (50) NOT NULL,
    [InclusionSum]     FLOAT (53)    NOT NULL,
    [Percents]         FLOAT (53)    NOT NULL,
    [Capitalize]       BIT           NOT NULL,
    [Total]            INT           NOT NULL,
    [StatusInclusion]  NVARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([ClientID]) REFERENCES [dbo].[Clients] ([Id])
);