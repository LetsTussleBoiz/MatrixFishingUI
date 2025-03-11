<lane orientation="vertical" horizontal-content-alignment="middle">
<!--Banner-->
    <banner layout="350px content"
        margin="16, 0"
        background={@Mods/StardewUI/Sprites/BannerBackground}
        background-border-thickness="48, 0"
        padding="12"
        text={Title} />
<!--Main UI-->
    <frame layout="240px content" padding="16" background={@Mods/Borealis.MatrixFishingUI/Sprites/cursors:HudBorder}>
<!--Currently Catchable Fish-->
        <grid layout="stretch content"
            item-layout="count: 4"
            item-spacing="16, 16"
            horizontal-item-alignment="start">
                <lane *repeat={LocalCatchableFish}>
                    <image layout="64px"
                        margin="0, 0, 0, 4"
                        sprite={ParsedFish} />
                </lane>
        </grid>
<!--Non-Catchable Fish-->
    </frame>
</lane>