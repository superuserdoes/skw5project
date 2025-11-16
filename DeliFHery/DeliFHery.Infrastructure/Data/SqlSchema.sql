CREATE TABLE Customers (
    Id INT IDENTITY PRIMARY KEY,
    IdentitySubjectId NVARCHAR(255) NOT NULL,
    DisplayName NVARCHAR(255) NOT NULL,
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NOT NULL
);

CREATE TABLE ContactMethods (
    Id INT IDENTITY PRIMARY KEY,
    CustomerId INT NOT NULL,
    Type NVARCHAR(50) NOT NULL,
    Value NVARCHAR(255) NOT NULL,
    IsPrimary BIT NOT NULL,
    IsVerified BIT NOT NULL,
    CreatedAt DATETIME2 NOT NULL,
    VerifiedAt DATETIME2 NULL,

    FOREIGN KEY (CustomerId) REFERENCES Customers(Id)
);
