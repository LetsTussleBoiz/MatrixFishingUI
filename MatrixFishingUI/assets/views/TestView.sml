<lane orientation="vertical" horizontal-content-alignment="middle">
    <lane vertical-content-alignment="middle">
        <image layout="48px 48px"
               horizontal-alignment="middle"
               vertical-alignment="middle"
               sprite={@Mods/StardewUI/Sprites/SmallLeftArrow}
               focusable="true"
               click=|PreviousFish()| />
        <banner layout="350px content"
                margin="16, 0"
                background={@Mods/StardewUI/Sprites/BannerBackground}
                background-border-thickness="48, 0"
                padding="12"
                text={HeaderText} />
        <image layout="48px 48px"
               horizontal-alignment="middle"
               vertical-alignment="middle"
               sprite={@Mods/StardewUI/Sprites/SmallRightArrow}
               focusable="true"
               click=|NextFish()| />
    </lane>
    <lane>
            <lane layout="150px content"
                  margin="0, 16, 0, 0"
                  orientation="vertical"
                  horizontal-content-alignment="end"
                  z-index="2">
                <frame *repeat={AllTabs}
                       layout="120px 64px"
                       margin={Margin}
                       padding="16, 0"
                       horizontal-content-alignment="middle"
                       vertical-content-alignment="middle"
                       background={@Mods/Borealis.MatrixFishingUI/Sprites/MenuTiles:TabButton}
                       focusable="true"
                       click=|^SelectTab(Value)|>
                    <label text={Value} />
                </frame>
            </lane>
            <frame *switch={SelectedTab}
                   layout="400px 300px"
                   margin="0, 16, 0, 0"
                   padding="32, 24"
                   background={@Mods/StardewUI/Sprites/ControlBorder}>
                <lane *case="General"
                      *context={:this}
                      layout="stretch content"
                      orientation="vertical"
                      horizontal-content-alignment="middle">
                    <label margin="0, 8" color="#136" text={Name} />
                    <lane orientation="vertical" horizontal-content-alignment="middle">
                        <image sprite={ParsedFish}
                            layout="64px" 
                            margin="0, 0, 0, 4" 
                            focusable="true"
                            transform-origin="0.5, 0.5"
                            +hover:transform="scale: 1.4"
                            +transition:transform="700ms EaseOutElastic"/>
                    </lane>
                    <lane orientation="horizontal" horizontal-content-alignment="start">
                        <label margin="0, 8" color="#136" text="Description: " />
                        <label margin="0, 8" color="#136" text={Description} />
                    </lane>
                    <lane orientation="horizontal" horizontal-content-alignment="start">
                        <label margin="0, 8" color="#136" text="Caught?: " />
                        <label margin="0, 8" color="#136" text={CaughtStatus} />
                    </lane>
                    <lane orientation="horizontal" horizontal-content-alignment="start">
                        <label margin="0, 8" color="#136" text="Number Caught: " />
                        <label margin="0, 8" color="#136" text={NumberCaught} />
                    </lane>
                </lane>
                <lane *case="CatchInfo" *context={:this} orientation="vertical">
                    <label margin="0, 8" color="#136" text="NOT IMPLEMENTED" />
                </lane>
                <lane *case="PondInfo"
                      *context={:this}
                      layout="stretch content"
                      orientation="vertical"
                      horizontal-content-alignment="middle">
                        <label margin="0, 8" color="#136" text="Pond Information" />
                        <grid layout="stretch"
                              item-layout="count: 4"
                              item-spacing="16, 16"
                              horizontal-item-alignment="middle">
                                <lane orientation="vertical" horizontal-content-alignment="middle">
                                    <image *repeat={ProducedItems}
                                           layout="64px" 
                                           margin="0, 0, 0, 4" 
                                           sprite={:this}
                                           tooltip={:this}
                                           focusable="true"
                                           transform-origin="0.5, 0.5"
                                           +hover:transform="scale: 1.4"
                                           +transition:transform="700ms EaseOutElastic"/>
                                </lane>
                        </grid>
                </lane>
            </frame>
            <spacer layout="50px content" />
    </lane>
</lane>
