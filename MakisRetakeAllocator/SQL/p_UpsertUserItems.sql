INSERT INTO maki_pistol_weapons
            (
                        steamid,
                        ct_secondary,
                        ct_armor,
                        ct_grenades,
                        ct_kit,
                        t_secondary,
                        t_armor,
                        t_grenades,
                        t_kit
            )
            VALUES
            (
                        @SteamId,
                        '@CTSecondary',
                        '@CTArmor',
                        '@CTGrenades',
                        @CTKit,
                        '@TSecondary',
                        '@TArmor',
                        '@TGrenades',
                        @TKit
            )
on duplicate KEY
UPDATE ct_secondary = '@CTSecondary',
       ct_armor = '@CTArmor',
       ct_grenades = '@CTGrenades',
       ct_kit = @CTKit,
       t_secondary = '@TSecondary',
       t_armor = '@TArmor',
       t_grenades = '@TGrenades',
       t_kit = @TKit;