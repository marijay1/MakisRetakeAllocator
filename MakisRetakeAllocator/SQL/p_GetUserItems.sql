SELECT 
    SteamId,
    ct_secondary,
    ct_armor,
    ct_grenades,
    ct_kit,
    t_secondary,
    t_armor,
    t_grenades,
    t_kit
FROM 
    maki_pistol_weapons
WHERE 
    SteamId = @SteamId;