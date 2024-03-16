CREATE TABLE IF NOT EXISTS maki_pistol_weapons (
    SteamId INT NOT NULL,
    ct_secondary VARCHAR(32) NOT NULL,
    ct_armor VARCHAR(32) NOT NULL,
    ct_grenades VARCHAR(128) NOT NULL,
    ct_kit BIT NOT NULL,
    t_secondary VARCHAR(32) NOT NULL,
    t_armor VARCHAR(32) NOT NULL,
    t_grenades VARCHAR(128) NOT NULL,
    t_kit BIT,
    CONSTRAINT unique_steam_id UNIQUE (SteamId)
);