<lane orientation="vertical" horizontal-content-alignment="middle">
    <banner background={@Mods/StardewUI/Sprites/BannerBackground} background-border-thickness="48,0" padding="12" text={:HeaderText} />
    <frame layout="400px 300px" background={@Mods/StardewUI/Sprites/ControlBorder} margin="0,16,0,0" padding="16">
        <scrollable peeking="128">
            <lane layout="stretch content"
                padding="16,8"
                orientation="vertical" 
                horizontal-content-alignment="middle">
                    <label bold="true" margin="0, 8" color="#136" text={#ui.special.title} font="dialogue" shadow-alpha="0.6" shadow-layers="VerticalAndDiagonal" shadow-offset="-3, 3" />
                    <lane *repeat={:Conditions} orientation="horizontal" horizontal-content-alignment="start">
                        <label bold="true" margin="0, 8" color="#136" text={#ui.special.segment} />
                        <label margin="0, 8" color="#136" text={:this} />
                    </lane>
            </lane>
		</scrollable>
    </frame>
</lane>