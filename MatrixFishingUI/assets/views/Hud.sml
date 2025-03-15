<lane orientation="vertical" horizontal-content-alignment="middle">
<!--Banner-->
    <banner layout="275px content"
        margin="16, 0"
        background={@Mods/StardewUI/Sprites/BannerBackground}
        background-border-thickness="48, 0"
        padding="12"
        text={Title} />
<!--Main UI-->
    <frame layout="300px content" padding="16" background={@Mods/Borealis.MatrixFishingUI/Sprites/cursors:HudBorder}>
<!--Currently Catchable Fish-->
        <lane *switch={IsThereFish} orientation="vertical" horizontal-content-alignment="middle" layout="stretch content">
            <lane *case="true" orientation="vertical" horizontal-content-alignment="middle" layout="stretch content">
                <grid layout="stretch content"
                    item-layout="count: 4"
                    item-spacing="16, 16"
                    horizontal-item-alignment="start">
                        <lane *repeat={:LocalCatchableFish} layout="stretch content">
                            <lane layout="64px 64px">
                                <image *if={HasBeenCaught}
                                    layout="64px"
                                    margin="0, 0, 0, 4"
                                    focusable="true"
                                    tooltip={:Name}
                                    sprite={:ParsedFish} />
                                <image *!if={HasBeenCaught}
                                    layout="64px"
                                    margin="0, 0, 0, 4"
                                    tint="#0006"
                                    focusable="true"
                                    tooltip={:Name}
                                    sprite={:ParsedFish} />
                            </lane>
                        </lane>
                </grid>
                <image layout="stretch content"
                    fit="stretch"
                    margin="0, 0, 0, 4"
                    padding="4"
                    sprite={@Mods/StardewUI/Sprites/ThinHorizontalDivider} />
<!--Non-Catchable Fish-->
                <grid layout="stretch content"
                    item-layout="count: 4"
                    item-spacing="16, 16"
                    horizontal-item-alignment="start">
                        <lane *repeat={:LocalUncatchableFish} layout="stretch content">
                            <lane layout="64px 64px">
                                <image *if={HasBeenCaught}
                                    layout="64px"
                                    margin="0, 0, 0, 4"
                                    focusable="true"
                                    tooltip={:Name}
                                    sprite={:ParsedFish} />
                                <image *!if={HasBeenCaught}
                                    layout="64px"
                                    margin="0, 0, 0, 4"
                                    focusable="true"
                                    tint="#0006"
                                    tooltip={:Name}
                                    sprite={:ParsedFish} />
                                <panel layout="stretch stretch" horizontal-content-alignment="end" vertical-content-alignment="start">
                                    <lane orientation="vertical" horizontal-content-alignment="end" vertical-content-alignment="start">
                                        <image *if={:BadTime} layout="18px 18px" sprite={@Mods/Borealis.MatrixFishingUI/Sprites/emojis:Clock} />
                                        <image *if={:BadSeason} layout="18px 18px" sprite={@Mods/Borealis.MatrixFishingUI/Sprites/emojis:Season} />
                                        <image *if={:BadWeather} layout="18px 18px" sprite={@Mods/Borealis.MatrixFishingUI/Sprites/emojis:Umbrella} />
                                    </lane>
                                </panel>
                            </lane>
                        </lane>
                </grid>
            </lane>
            <lane *case="false" orientation="vertical" horizontal-content-alignment="middle" >
                <label margin="0, 8" color="#136" text={#ui.hud.labels.no_fish} />
            </lane>
        </lane>
    </frame>
</lane>