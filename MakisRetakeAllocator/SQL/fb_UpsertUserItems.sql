INSERT INTO maki_fullbuy_weapons
            (
                        steamid,
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
            )
            VALUES
            (
                        @SteamId,
                        '@CTPrimary',
                        '@CTSecondary',
                        '@CTArmor',
                        '@CTGrenades',
                        @CTKit,
                        '@TPrimary',
                        '@TSecondary',
                        '@TArmor',
                        '@TGrenades',
                        @TKit
            )
on duplicate KEY
UPDATE ct_primary = '@CTPrimary',
       ct_secondary = '@CTSecondary',
       ct_armor = '@CTArmor',
       ct_grenades = '@CTGrenades',
       ct_kit = @CTKit,
       t_primary = '@TPrimary',
       t_secondary = '@TSecondary',
       t_armor = '@TArmor',
       t_grenades = '@TGrenades',
       t_kit = @TKit;