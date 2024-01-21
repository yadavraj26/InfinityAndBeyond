# Infinity and Beyond
Infinity and Beyond is a game simulation project to explore Procedural Content Generation and Interactive Agents. Cellular Automata, Behaviour Tree, and Utility-based Agents have been explored. The game simulation takes the theme of Starcraft. Three Races Protoss, Terran and Zerg battle in a galaxy filled with asteroid fields. The level is generated using Cellular Automata, and the starships are controlled by Utility-based Agents making use Behaviour Tree.

# Level Generator - Generator.cs
The level generator works on the cellular automata concept.
The rules followed for the cellular automata are if surrounding blacks < 4 - spawn white or if >4 - spawn black, else stays the same.
An intensive for loop is initially run over the canvas[width, height] and spawns random blacks and whites in the canvas. Then  to make sure a generation has circular shapes similar to planets, the function create planet was used to place some random circular elements on the canvas. Cellular automata rules are applied in the function 'ApplyCellularAutomata'. 15 consecutive iterations are carried over to get a good map.

# Agent 1 - Protoss.cs
Behaviour:
1) Guard Planet
2) Attack enemies if in the vicinity and Flee if losing the battle. Make sure to target Zerg more than Terran
3) Heal from the sun if health lowers or wanders as per the state of the agent.

The agent uses a combined method of utility and Behaviour trees. The methods or states are

State- Function defining the state
GuardPlanet - GuardPlanetBehaviour() 
Attack - AttackBehaviour()
Flee - FleeBehaviour()
Soak in Sun - SoakInSunBehaviour()

# Agent 2 - ZergStarship.cs
Behaviour
1) Destroy Planet
2) Attack if in vicinity( No partiality and never flees)
3) Heal from the planet if the planet spotted
4) Flee from the sun if near

The agent uses a combined method of utility and Behaviour trees. The methods or states are
Destroy - DestroyPlanetBehaviour()
Attack - AttackBehaviour()
Wander - WanderBehaviour()
SoakinPlanet - Soak in Planet behaviour()
Flee- FleeBehaviour()

# Agent 3 - Terran.cs
behaviour
1) Hide and attack only when you know you will win the battle
2) Ally with protoss in war with Zerg
3) Gather firepower from the planet also heal and develop the planet by shooting bullets of development.
The agent uses a combined method of utility and Behaviour trees. The methods or states are

Wander - WanderBehaviour()
Attack - AttackBehaviour()
Flee -  FleeBehaviour()
HealPlanet - HealPlanetBehaviour()
