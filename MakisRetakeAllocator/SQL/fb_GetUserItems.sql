SELECT 
    SteamId,
    ct_primary,
    ct_secondary,
    ct_armor,
    ct_grenades,
    ct_kit,
    t_primary,
    t_secondary,
    t_armor,
    t_grenades,
    t_kit
FROM 
    maki_fullbuy_weapons
WHERE 
    SteamId = @SteamId;