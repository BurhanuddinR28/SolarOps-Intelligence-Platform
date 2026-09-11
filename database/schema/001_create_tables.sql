Create table Plants(
    PlantID INT identity(1,1) primary KEY,
    PlantName VARCHAR(100) NOT NULL
);

CREATE TABLE Tools(
    ToolID INT identity(1,1) primary KEY,
    ToolName VARCHAR(100) NOT NULL,
    ToolLine VARCHAR(100) NOT NULL
);
CREATE TABLE PanelReadings (
    ReadingId   INT IDENTITY(1,1) PRIMARY KEY,
    SubId       VARCHAR(50) NOT NULL,
    Timestamp   DATETIME NOT NULL,
    ReadTime    FLOAT NULL,
    Wattage     FLOAT NULL,
    PlantId     INT NOT NULL,
    ToolId      INT NOT NULL,
    PassFail    BIT NULL,
    CreatedAt   DATETIME NOT NULL,
    FOREIGN KEY (PlantId) REFERENCES Plants(PlantId),
FOREIGN KEY (ToolId) REFERENCES Tools(ToolId)
);
CREATE TABLE IVCurves(
    IVId INT Identity(1,1) PRIMARY KEY,
    ReadingId INT NOT NULL,
    FOREIGN KEY (ReadingId) REFERENCES PanelReadings(ReadingId),
    Voltage FLOAT,
    Current FLOAT
)