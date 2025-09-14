<lane orientation="vertical" horizontal-content-alignment="middle">
    <lane vertical-content-alignment="middle">
        <image layout="48px 48px"
            horizontal-alignment="middle"
            vertical-alignment="middle"
            sprite={@Mods/StardewUI/Sprites/SmallLeftArrow}
            focusable="true"
            click=|PreviousFish()| />
        <banner layout="450px content"
            margin="16, 0"
            background={@Mods/StardewUI/Sprites/BannerBackground}
            background-border-thickness="48, 0"
            padding="12"
            text={:Name} />
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
                <frame *repeat={:AllTabs}
                    layout="120px 64px"
                    margin={Margin}
                    padding="16, 0"
                    horizontal-content-alignment="middle"
                    vertical-content-alignment="middle"
                    background={@Mods/Borealis.MatrixFishingUI/Sprites/MenuTiles:TabButton}
                    focusable="true"
                    click=|^SelectTab(Value)|>
                        <label text={:Value} />
                </frame>
        </lane>
        <frame *switch={SelectedTab}
            layout="800px 600px"
            margin="0, 16, 0, 0"
            padding="32, 24"
            background={@Mods/StardewUI/Sprites/ControlBorder}>
                <lane *case="General"
                    layout="stretch content"
                    orientation="vertical"
                    horizontal-content-alignment="middle">
                        <label bold="true" margin="0, 8" color="#136" text={#ui.fishipedia.titles.general} font="dialogue" shadow-alpha="0.6" shadow-layers="VerticalAndDiagonal" shadow-offset="-3, 3"/>
                        <lane layout="64px 64px"
                            focusable="true"
                            transform-origin="0.5, 0.5"
                            +hover:transform="scale: 1.4"
                            +transition:transform="700ms EaseOutElastic">
                            <image sprite={:ParsedFish}
                                layout="64px" 
                                margin="0, 0, 0, 4" 
                                tooltip={:ParsedFish} /> 
                            <panel layout="stretch stretch" horizontal-content-alignment="end" vertical-content-alignment="start">
                                <image *if={:Legendary} 
                                    layout="24px" 
                                    tooltip={#ui.fishipedia.tooltips.legendary} 
                                    sprite={@Mods/Borealis.MatrixFishingUI/Sprites/cursors:Legendary} />
                            </panel>
                        </lane>
                        <lane orientation="horizontal" horizontal-content-alignment="start">
                            <label bold="true" margin="0, 8" color="#136" text={#ui.fishipedia.labels.description} />
                            <label margin="0, 8" color="#136" text={:Description} />
                        </lane>
                        <lane orientation="horizontal" horizontal-content-alignment="middle">
                            <label bold="true" margin="0, 8" color="#136" text={#ui.fishipedia.labels.caught_status} />
                            <label margin="0, 8" color="#136" text={:CaughtStatus} />
                        </lane>
                        <lane orientation="horizontal" horizontal-content-alignment="middle">
                            <label bold="true" margin="0, 8" color="#136" text={#ui.fishipedia.labels.min_size} />
                            <label margin="0, 8" color="#136" text={:MinSize} />
                            <spacer layout="4px 0px" />
                            <label margin="0, 8" color="#136" text={#ui.fishipedia.labels.inches} />
                        </lane>
                        <lane orientation="horizontal" horizontal-content-alignment="middle">
                            <label bold="true" margin="0, 8" color="#136" text={#ui.fishipedia.labels.max_size} />
                            <label margin="0, 8" color="#136" text={:MaxSize} />
                            <spacer layout="4px 0px" />
                            <label margin="0, 8" color="#136" text={#ui.fishipedia.labels.inches} />
                        </lane>
                        <lane orientation="horizontal" horizontal-content-alignment="start">
                            <label bold="true" margin="0, 8" color="#136" text="Special Information: " />
                            <label margin="0, 8" color="#136" text={:SpecialInfo} />
                        </lane>
                </lane>
                <scrollable peeking="128" *case="CatchInfo">
                <lane orientation="vertical" horizontal-content-alignment="middle" layout="stretch content">
                    <lane *switch={:FishType} orientation="vertical" horizontal-content-alignment="middle">
                        <lane *case="Trap" orientation="vertical" horizontal-content-alignment="middle">
                            <label bold="true" margin="0, 8" color="#136" text={#ui.fishipedia.titles.trap} font="dialogue" shadow-alpha="0.6" shadow-layers="VerticalAndDiagonal" shadow-offset="-3, 3"/>
                            <lane orientation="horizontal" horizontal-content-alignment="start">
                                <label bold="true" margin="0, 8" color="#136" text={#ui.fishipedia.labels.water_type} />
                                <label margin="0, 8" color="#136" text={:WaterType} />
                            </lane>
                        </lane>
                        <lane *case="Catch" orientation="vertical" horizontal-content-alignment="middle">
                            <label bold="true" margin="0, 8" color="#136" text={#ui.fishipedia.titles.catch} font="dialogue" shadow-alpha="0.6" shadow-layers="VerticalAndDiagonal" shadow-offset="-3, 3"/>
                            <lane orientation="horizontal" horizontal-content-alignment="middle">
                                <label bold="true" margin="0, 8" color="#136" text={#ui.fishipedia.labels.fish_caught} />
                                <label margin="0, 8" color="#136" text={:NumberCaught} />
                            </lane>
                            <lane orientation="vertical" horizontal-content-alignment="middle">
                                <label bold="true" margin="0, 8" color="#136" text={#ui.fishipedia.time.time_available} />
                                <lane *repeat={:Times} orientation="horizontal" horizontal-content-alignment="middle">
                                    <label margin="0, 8" color="#136" text={#ui.fishipedia.time.between} />
                                    <label margin="0, 8" color="#136" text={:StartTime} />
                                    <label margin="0, 8" color="#136" text={#ui.fishipedia.time.and} />
                                    <label margin="0, 8" color="#136" text={:EndTime} />
                                </lane>
                            </lane>
                            <lane orientation="horizontal" horizontal-content-alignment="middle">
                                <label bold="true" margin="0, 8" color="#136" text={#ui.fishipedia.labels.fishing_level} />
                                <label margin="0, 8" color="#136" text={:MinLevel} />
                            </lane>
                            <lane orientation="horizontal" horizontal-content-alignment="middle">
                                <label bold="true" margin="0, 8" color="#136" text={#ui.fishipedia.labels.biggest} />
                                <label margin="0, 8" color="#136" text={:BiggestCatch} />
                            </lane>
                            <lane orientation="horizontal" horizontal-content-alignment="middle">
                                <label bold="true" margin="0, 8" color="#136" text={#ui.fishipedia.labels.difficulty} />
                                <label margin="0, 8" color="#136" text={:Difficulty} />
                                <spacer layout="4px content" />
                                <label margin="0, 8" color="#136" text={:DifficultyType} />
                            </lane>
                            <lane orientation="horizontal" horizontal-content-alignment="middle">
                                <label bold="true" margin="0, 8" color="#136" text={#ui.fishipedia.labels.weather_required} />
                                <lane *switch={:FishWeather} orientation="vertical">
                                    <image *case="Sunny"
                                        layout="104px 40px"
                                        horizontal-alignment="middle"
                                        vertical-alignment="middle"
                                        sprite={@Mods/Borealis.MatrixFishingUI/Sprites/cursors:Sunny}
                                        tooltip={#ui.fishipedia.weather.sunny}
                                        focusable="true" />
                                    <image *case="Rain"
                                        layout="104px 40px"
                                        horizontal-alignment="middle"
                                        vertical-alignment="middle"
                                        sprite={@Mods/Borealis.MatrixFishingUI/Sprites/cursors:Rainy}
                                        tooltip={#ui.fishipedia.weather.rainy}
                                        focusable="true" />
                                    <image *case="Any"
                                        layout="104px 40px"
                                        horizontal-alignment="middle"
                                        vertical-alignment="middle"
                                        sprite={@Mods/Borealis.MatrixFishingUI/Sprites/cursors:Any}
                                        tooltip={#ui.fishipedia.weather.any}
                                        focusable="true" />
                                    <lane *!case="Sunny">
                                        <lane *!case="Rain">
                                            <image *!case="Any"
                                                layout="104px 40px"
                                                horizontal-alignment="middle"
                                                vertical-alignment="middle"
                                                sprite={@Mods/Borealis.MatrixFishingUI/Sprites/cursors:None}
                                                tooltip={:FishWeather}
                                                focusable="true" />
                                        </lane>
                                    </lane>
                                </lane>
                            </lane>
                            <lane orientation="vertical" horizontal-content-alignment="middle">
                                <label bold="true" margin="0, 8" color="#136" text={#ui.fishipedia.titles.locations} font="dialogue" shadow-alpha="0.6" shadow-layers="VerticalAndDiagonal" shadow-offset="-3, 3"/>
                                <lane *repeat={:LocationSeasonPairs} orientation="vertical" horizontal-content-alignment="start">
                                    <lane orientation="horizontal">
                                        <lane *context={:Location}>
                                            <label bold="true" margin="0, 8" color="#136" text={:LocationReadableName} />
                                        </lane>
                                        <spacer layout="4px 0px" />
                                            <label margin="0, 8" color="#136" text="during" />
                                        <spacer layout="4px 0px" />
                                        <lane *repeat={:Seasons}> 
                                            <lane *switch={:this}>
                                                <image *case="Spring" tooltip={#ui.fishipedia.season.spring} layout="60px 40px" sprite={@Mods/Borealis.MatrixFishingUI/Sprites/cursors:Spring} />
                                                <image *case="Summer" tooltip={#ui.fishipedia.season.summer} layout="60px 40px" sprite={@Mods/Borealis.MatrixFishingUI/Sprites/cursors:Summer} />
                                                <image *case="Fall" tooltip={#ui.fishipedia.season.fall} layout="60px 40px" sprite={@Mods/Borealis.MatrixFishingUI/Sprites/cursors:Fall} />
                                                <image *case="Winter" tooltip={#ui.fishipedia.season.winter} layout="60px 40px" sprite={@Mods/Borealis.MatrixFishingUI/Sprites/cursors:Winter} />
                                            </lane>
                                        </lane>
                                        <spacer layout="4px 0px" />
                                        <lane *if={:HasSpecialConditions} *context={:Location}>
                                            <image layout="15px 42px"
                                                horizontal-alignment="middle"
                                                vertical-alignment="middle"
                                                sprite={@Mods/Borealis.MatrixFishingUI/Sprites/cursors:Exclamation}
                                                tooltip="Special Condition(s)"
                                                focusable="true"
                                                click=|^^ViewConditions(LocationName)| />
                                        </lane>
                                    </lane>
                                </lane>
                            </lane>
                        </lane>
                    </lane>
                </lane>
                </scrollable>
                <scrollable peeking="128" *case="PondInfo" layout="stretch">
                <lane layout="stretch content"
                    orientation="vertical"
                    horizontal-content-alignment="middle">
                        <label bold="true" margin="0, 8" color="#136" text={#ui.fishipedia.titles.pond} font="dialogue" shadow-alpha="0.6" shadow-layers="VerticalAndDiagonal" shadow-offset="-3, 3"/>
                        <label margin="0, 8" color="#136" text={:SpawnTimeString} />
                        <label bold="true" margin="0, 8" color="#136" text={#ui.fishipedia.titles.items_produced} font="dialogue" shadow-alpha="0.6" shadow-layers="VerticalAndDiagonal" shadow-offset="-3, 3"/>
                        <grid *context={:PondItems}
                            layout="stretch content"
                            item-layout="count: 1"
                            item-spacing="8, 16"
                            horizontal-item-alignment="start"
                            padding="32, 0" >
                                <lane *repeat={:ProducedItems} orientation="vertical" horizontal-content-alignment="start">
                                    <lane orientation="horizontal" horizontal-content-alignment="start" vertical-content-alignment="middle">
                                        <lane *!if={:IsRoe} layout="96px 96px"                                                 
                                            focusable="true"
                                            transform-origin="0.5, 0.5"
                                            +hover:transform="scale: 1.4"
                                            +transition:transform="700ms EaseOutElastic" >
                                                <panel layout="96px 96px" horizontal-content-alignment="end" vertical-content-alignment="end">
                                                    <image layout="96px"
                                                        margin="0, 0, 0, 4"
                                                        sprite={:Item}
                                                        tooltip={:Item} />
                                                    <lane orientation="horizontal" horizontal-content-alignment="middle">
                                                        <label bold="true" text={:QuantityString} color="#FFFFFF" font="small" shadow-alpha="0.8" shadow-layers="VerticalAndDiagonal" shadow-offset="-3, 3"/>
                                                    </lane>
                                                </panel>
                                        </lane>
                                        <lane *if={:IsRoe} layout="96px 96px"                                                 
                                            focusable="true"
                                            transform-origin="0.5, 0.5"
                                            +hover:transform="scale: 1.4"
                                            +transition:transform="700ms EaseOutElastic" >
                                                <panel layout="96px 96px" horizontal-content-alignment="end" vertical-content-alignment="end">
                                                    <image layout="96px"
                                                        margin="0, 0, 0, 4"
                                                        sprite={@Mods/Borealis.MatrixFishingUI/Sprites/springobjects:WhiteRoe}
                                                        tooltip={:FlavoredRoe}
                                                        tint={:RoeColor} />
                                                    <lane orientation="horizontal" horizontal-content-alignment="middle">
                                                        <label bold="true" text={:QuantityString} color="#FFFFFF" font="small" shadow-alpha="0.8" shadow-layers="VerticalAndDiagonal" shadow-offset="-3, 3"/>
                                                    </lane>
                                                </panel>
                                        </lane>
                                        <label margin="0, 8" color="#136" text={:Chance} />
                                        <spacer layout="4px 0px" />
                                        <label margin="0, 8" color="#136" text={:SalesPrice} />
                                        <label margin="0, 8" color="#136" text={:PopulationRequired} />
                                    </lane>
                                </lane>
                        </grid>
                </lane>
            </scrollable>
        </frame>
        <spacer layout="50px content" />
    </lane>
</lane>
