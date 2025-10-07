<lane orientation="vertical" horizontal-content-alignment="middle">
    <banner background={@Mods/StardewUI/Sprites/BannerBackground} background-border-thickness="48,0" padding="12" text={:HeaderText} />
    <frame layout="550px 300px" background={@Mods/StardewUI/Sprites/ControlBorder} margin="0,16,0,0" padding="16">
        <scrollable peeking="128">
            <lane layout="stretch content"
                padding="4,8"
                orientation="vertical" 
                horizontal-content-alignment="middle">
                    <label bold="true" margin="0, 8" color="#136" text={#ui.area.title} font="dialogue" shadow-alpha="0.6" shadow-layers="VerticalAndDiagonal" shadow-offset="-3, 3" />
                    <lane orientation="horizontal">
                        <label bold="true" margin="0, 8" color="#136" text={#ui.area.segment} />
                        <label margin="0, 8" color="#136" text={AreaName} />
                    </lane>
            </lane>
		</scrollable>
    </frame>
</lane>