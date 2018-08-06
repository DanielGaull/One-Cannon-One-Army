using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneCannonOneArmy
{
    public static class LanguageTranslator
    {
        #region Spanish
        static Dictionary<string, string> spanishDict = new Dictionary<string, string>()
        {
            { "start", "Comienza" },
            { "quit", "Deja" },
            { "play", "Juega" },
            { "achievements", "Logros" },
            { "stats", "Estadísticas" },
            { "upgrade", "Mejora" },
            { "shop", "Tienda" },
            { "crafting", "Elaboración" },
            { "organize", "Organiza" },
            { "back", "Regresa" },
            { "create new user", "Crea nuevo usario" },
            { "create user", "Crea usario" },
            { "main menu", "Menú principal" },
            { "resume", "Continua" },
            { "skip", "Se salta" },
            { "gifts", "Regalos" },
            { "submit", "Entrega" },
            { "replay tutorial", "Repite el tutorial" },
            { "credits", "Créditos" },

            { "buy", "Compra" },
            { "sell", "Vende" },
            { "sell all", "Vende todos" },
            { "sell value", "Vender valor" },
            { "craft", "Crea" },
            { "you have", "Tiene" },
            { "cost", "Costo" },
            { "select", "Selecciona" },
            { "reward", "Recompensa" },
            { "coins", "Monedas" },
            { "time until next life: ", "Tiempo hasta la próxima vida: " },
            { "next free gift in", "Siguiente regalo gratis en" },

            { "materials", "Materiales" },
            { "cannons", "Cañones" },

            { "red", "Rojo" },
            { "orange", "Naranja" },
            { "yellow", "Amarillo" },
            { "green", "Verde" },
            { "blue", "Azul" },
            { "purple", "Morado" },
            { "gift", "Regalo" },

            { "open", "Abre" },
            { "claim gift", "Colecciona regalo" },
            { "click to go faster.", "Haga clic para ir más rápido." },
            { "you've received a free {0} gift!", "¡Has recibido un regalo {0}!" },

            { "username", "Nombre" },
            { "brightness", "El brillo" },
            { "system", "Sistema" },
            { "music", "Música" },
            { "sound effects", "Efectos de sonido" },
            { "are you sure? the program will restart and the tutorial and story will be replayed.",
                "¿Estás seguro? Se reanudará el programa y el tutorial y el cuento se repetirá." },
            { "many sound issues can be resolved by reinstalling directx. click \"okay\" to " +
                "open the install\npage in your browser. be sure to sure to close all programs and " +
                "restart your computer after the\ninstall is complete.",
                "Pueden resolver muchos problemas de sonido reinstalando DirectX. Haga clic en \"Continúa\"\npara " +
                "abrir la página de instalación en su navegador. Asegúrese de que para cerrar todos los\nprogramas y " +
                "reinicie el equipo después de completar la instalación." },
            { "sound issues?", "¿Problemas de sonido?" },

            { "health", "Salud" },
            { "damage", "Daño" },
            { "move speed", "Velocidad de movimiento" },
            { "reload speed", "Velocidad de recarga" },
            { "accuracy", "Precisión" },
            { "rapid fire", "Lanzamiento rápido" },
            { "max level", "Nivel máximo" },

            { "bronze", "Bronce" },
            { "silver", "Plata" },
            { "gold", "Oro" },
            { "elite", "Élite" },
            { "inferno", "Infierno" },
            { "frozen", "Congelado" },

            { "rock", "Rock" },
            { "cannonball", "Bala de cañón" },
            { "dart", "Dardo" },
            { "poison dart", "Dardo venenoso" },
            { "fireball", "Bola de fuego" },
            { "bomb", "Bomba" },
            { "laser", "Láser" },
            { "hex", "Maleficio" },
            { "lightning bolt", "Rayo" },
            { "frost hex", "Explosión congelada" },
            { "meteor", "Meteorito" },
            { "hammer", "Martillo" },
            { "rocket", "Cohete" },
            { "poison rocket", "Cohete venenoso" },
            { "fire rocket", "Cohete flameante" },
            { "frozen rocket", "Cohete congelada" },
            { "plasma rocket", "Cohete de plasma" },
            { "omega rocket", "Cohete omega" },
            { "snowball", "Bola de nieve" },
            { "shuriken", "Shuriken" },
            { "ice shard", "Fragmento de hielo" },
            { "bone", "Hueso" },

            { "stone", "Piedra" },
            { "metal", "Metal" },
            { "poison", "Veneno" },
            { "gunpowder", "Pólvora" },
            { "ice", "Hielo" },
            { "essence of fire", "Fuego" },
            { "chaos energy", "Caos" },
            { "plasma", "Plasma" },

            { "okay", "Continúa" },
            { "cancel", "Cancela" },
            { "do not show again.", "No muestra de nuevo." },

            { "buy one life for ", "Compra una vida por " },
            { "are you sure you want to quit?", "¿Está seguro que quiere salir?" },
            { "you do not have enough coins.", "No tiene suficientes monedas." },
            { "please enter a username containing at least one non-whitespace character.",
                "Introduce un nombre de usuario que contiene al menos un carácter no está en blanco." },
            { "that user already exists.", "Eso usario ya existe." },
            { "are you sure you want to delete", "¿Está seguro que desea eliminar" },
            { "you will be charged with one life.", "Perderá una vida." },
            { "you've earned the \nachievement \"", "¡Ha completado el \nlogro \"" },
             { "the shortcut ctrl+w exits the game. are you sure you want to do this?" ,
                 "El atajo Ctrl+W se cierra el juego. ¿Está seguro que quiere hacer esto?" },
            { "this level requires about {0} damage to beat and you only have about {1} worth of\ndamage in your inventory. continue?", "Este nivel requiere un daño de {0} a latir y sólo tiene un valor de {1} de daño\nen su inventario. Continuar?" },

            { "aliens killed", "Extraterrestres asesinados" },
            { "aliens hit", "Extraterrestres golpeados" },
            { "coins collected", "Monedas recolectadas" },
            { "achievements completed", "Logros terminados" },
            { "projectiles fired", "Proyectiles disparados" },
            { "coins spent", "Mondedas gastadas" },
            { "total purchases", "Compras totales" },
            { "items crafted", "Artículos artesanales" },
            { "items sold", "Artículos vendidos" },
            // "accuracy" is defined above

            { "gettin' warmed up", "Introducción" },
            { "stay off my planet!", "¡Manténgase fuera de mi planeta!" },
            { "alien slayer", "Asesino de extraterrestres" },
            { "master of the cannon", "El maestro del cañón" },
            { "defender of earth", "El defensor de la tierra" },
            { "boom, boom", "Boom, boom" },
            { "trigger happy", "Gatillo fácil" },
            { "fire in the hole!", "¡Dispara en el agujero!" },
            { "one cannon but no army", "Un cañón pero ningún ejército" },
            { "gimme those rocks", "Me dan esas rocas" },
            { "big spender", "Gastador grande" },
            { "shop addict", "Adicto de la tienda" },
            { "serial spender", "Comprador en serie" },
            { "builder", "El albañil" },
            { "artisan", "El artesano" },
            { "blacksmith", "El herrero" },
            { "craftoholic", "Elaborahólico" },

            { "kill an alien.", "Mata a un extraterrestre." },
            { "kill 25 aliens.", "Mata a 25 extraterrestres." },
            { "kill 50 aliens.", "Mata a 50 extraterrestres." },
            { "kill 100 aliens.", "Mata a 100 extraterrestres." },
            { "kill 500 aliens.", "Mata a 500 extraterrestres." },
            { "fire 50 projectiles.", "Dispara a 50 proyetiles." },
            { "fire 100 projectiles.", "Dispara a 100 proyetiles." },
            { "fire 500 projectiles.", "Dispara a 500 proyetiles." },
            { "defeat malos.", "Derrota Malos." },
            { "spend 100 coins.", "Gasta 100 mondedas." },
            { "spend 250 coins.", "Gasta 250 mondedas." },
            { "spend 500 coins.", "Gasta 500 mondedas." },
            { "spend 1,000 coins.", "Gasta 1,000 mondedas." },
            { "craft 10 items.", "Elabora 10 artículos." },
            { "craft 50 items.", "Elabora 50 artículos." },
            { "craft 100 items.", "Elabora 100 artículos." },
            { "craft 500 items.", "Elabora 500 artículos." },

            { "kill all the aliens!", "Mata a todos los extraterrestres!" },
            { "destroy {0} aliens carrying mechanical supplies!", "¡Destruye a 5 extranjeros que llevan suministros mecánicos!" },
            { "break open {0} cages!", "¡Abre {0} jaulas!" },
            { "break open {0} cage!", "¡Abre {0} jaula!" },
            { "defeat the aliens, then destroy the laser!", "¡Derrota a los extraterrestres, y luego destruye el laser!" },
            { "kill malos!", "¡Mata a Malos!" },

            { "complete previous missions first.", "Completa a las misiones anteriores primero." },
            { "tutorial", "El tutorial" },
            { "rise of the aliens", "El auge de los extraterrestres" },
            { "rome was not built in one day", "Roma no se construyó en un día" },
            { "hat trick", "Broma del sombrero" },
            { "the last straw", "La última gota" },
            { "hit two aliens with one stone", "Golpeó a dos extraterrestres de un tiro" },
            { "back to the drawing board", "Volviendo a la pizarra" },
            { "armored assault", "Asalto armado" },
            { "it's a small world", "Es un pequeño mundo" },
            { "breaking ice", "Rompe hielo" },
            { "spell game", "Hechizo juego" },
            { "battle of boston", "Batalla de Boston" },
            { "skirmish in san antonio", "Escaramuza en San Antonio" },
            { "chicago combat", "Combate de Chicago" },
            { "liberation of los angeles", "Liberación de Los Angeles" },
            { "d.c. defense", "Defensa de D.C." },
            { "seoul duel", "Duelo de Seoul" },
            { "clash of sao paulo", "Choque de Sao Paulo" },
            { "strike against new york", "Huelga contra New York" },
            { "tokyo invasion", "Invasión de Tokyo" },
            { "london warfare", "Guerra en London" },
            { "liftoff", "Despegue" },
            { "attack on nantak", "Ataque en Nantak" },
            { "attack on carinus", "Ataque en Carinus" },
            { "attack on mikara", "Ataque en Mikara" },
            { "attack on lithios", "Ataque en Lithios" },

            { "you've completed the tutorial!", "¡Ha completado el tutorial!" },
            { "you've saved the entire planet!\nthe world is in your debt.", "¡Ha salvado el planeta! El mundo está en su deuda." },
            { "you've reached the lambda romana star system!\nonward, to nantak!",
                "¡Ha llegado el sistema Lambda Romana!\n¡Adelante, a Nantak!" },
            { "one down, two to go.\nonward, to carinus!", "Uno se ha ido, dos permanecen.\n¡Adelante, a Carinus!" },
            { "only one laser left.\ntime to go to mikara.", "Sólo un láser permanece. Es el momento de ir a Mikara." },
            { "good job destroying those lasers!\nnow it's time to battle malos himself. " +
                            "to lithios!",
                "¡Buen trabajo destruyendo esos lasers! " +
                "Ahora es tiempo de batalla Malos sí mismo. ¡A Lithios!" },
            { "excellent job! \nyou've saved", "¡Excelente trabajo! Ha salvado" },
            { " ... again!", " ... ¡de nuevo!" },
            { " ... for the third time!", " ... ¡por tercera vez!" },

            { "new alien", "Nuevo extraterrestre" },
            { "new projectile", "Nuevo proyectil" },
            { "alien shields", "Escudos de los extraterrestres" },

            { "normal alien", "Extraterrestre normal" },
            { "light defense alien", "Extraterrestre de defensa ligero" },
            { "defense alien", "Extraterrestre de defensa" },
            { "heavy defense alien", "Extraterrestre de defensa fuerte" },
            { "light fire resistant alien", "Extraterrestre de ignifugo ligero" },
            { "fire resistant alien", "Extraterrestre de ignifugo" },
            { "heavy fire resistant alien", "Extraterrestre de ignifugo fuerte" },
            { "light poison resistant alien", "Extraterrestre de veneno resistente ligero" },
            { "poison resistant alien", "Extraterrestre de veneno resistente" },
            { "heavy poison resistant alien", "Extraterrestre de veneno resistente fuerte" },
            { "light plasma resistant alien", "Extraterrestre de plasma resistente ligero" },
            { "plasma resistant alien", "Extraterrestre de plasma resistente" },
            { "heavy plasma resistant alien", "Extraterrestre de plasma resistente fuerte" },
            { "chaos alien", "Extraterrestre de caos" },
            { "omega alien", "Extraterrestre omega" },
            { "malos", "Malos" },

            { "move right", "Mueve a la derecha" },
            { "move left", "Mueve a la izquierda" },
            { "launch", "Disparo" },
            { "sweep", "Barre" },
            { "pause", "Pausa" },
            { "fullscreen", "Pantalla completa" },
            { "screenshot", "Toma pantallazo" },
            { "hotbar 1", "Tablero 1" },
            { "hotbar 2", "Tablero 2" },
            { "hotbar 3", "Tablero 3" },
            { "hotbar 4", "Tablero 4" },
            { "hotbar 5", "Tablero 5" },

            { "toggle rapid fire", "Activar disparar rápido" },
            { "sweep (collect all alien drops)", "Barrido (recoger todas las gotas extraterrestre)" },
            { "(upgrade not purchased)", "(Actualización no adquirido)" },

            #region Item Desc. Translations

            { "almost-useless lump of earth. can craft useful\nthings, though.", "Casi-inútil trozo de tierra. Puede crear cosas\nútiles, aunque" },
            { "slightly less-useless lump of earth. basic\nmaterial; used to craft many powerful projectiles.",
              "Un poco menos inútil terrón de tierra. Material básico;\nutiliza para elaborar numerosos proyectiles de gran alcance." },
            { "collected from plants. can be used for\nmaking poison, or healing concoctions",
              "Recolectadas de las plantas. Puede ser\nutilizado para hacer el veneno, o los\nbrebajes curativos" },
            { "powerful red energy, used for magic.", "Energía rojo de gran alcance, utilizada para magia." },
            { "it can keep drinks cold, help you cool off on a hot\nday, and freeze aliens! that ice... so useful.",
                "Puede mantener las bebidas frías, ayudarle a refrescarse en un\ndía caluroso y la congelación de los extraterrestres! " +
                "Que hielo...\ntan útil." },
            { "explosive powder. when mixed with fire...look out.",
                "Polvo explosivo. Cuando está mezclado con fuego...Tener cuidado." },
            { "almost uncontainable magical energy. if you can\ncontain it, magic! if you can't... more dangerous\nmagic.",
                "Casi incontenible energía mágica. Si usted puede\ncontenerla, ¡mágico! Si no puedes... más peligrosa\nmagia." },
            { "magical fiery energy. great for barbecuing!", "Energía mágica de fuego. ¡Ideal para barbacoa!" },

            { "just your basic cannon.", "Sólo un cañón básico." },
            { "reinforced with bronze for more damage.", "Reforzado con bronce para más daño." },
            { "stronger, silver cannon. increased speed,\ndamage, and accuracy.",
                "Más fuerte cañón de la plata. Aumento de la velocidad, daño y\nprecisión." },
            { "who's idea was this!? this is an expensive,\nthough powerful, cannon.",
                "¿De quién fue la idea de este? Se trata de un costoso, aunque\npotente, cañón." },
            { "it's quick, it's accurate, it's powerful!", "Es rápido, es precisa, es de gran alcance!" },
            { "burn everything with this cannon! just don't set\nthe lawn on fire...",
                "¡Quemar todo con este cañón! Sólo no incendiaron el césped..." },
            { "adds a bit of ice to each projectile to make\nthem freeze aliens!",
                "¡Añade un poco de hielo a cada proyectil para congelar a aliens!" },

            { "though weak, rocks are cheap and easy to make.", "Aunque débil, las rocas son baratas y fáciles de hacer." },
            { "slightly stronger though more expensive than the rock.", "Ligeramente más fuerte, aunque más caro que la roca." },
            { "burn enemies or cook up some steak; gotta love\nthat fireball.",
                "Quemar a los enemigos o cocinar un poco de carne; tengo que\namar esa bola de fuego." },
            { "explodes on impact. don't try this at home.", "Explota en el impacto. No intentes esto en casa." },
            { "quick and dangerous.", "Es rápido y peligroso." },
            { "the poison tip makes it much more deadly than a\nplain dart.",
                "La punta envenenada lo hace mucho más letal que un dardo\nnormal." },
            { "slows aliens and deals light damage.", "Ralentiza a los extraterrestres y inflige daño ligero." },
            { "pew! pew! fire a powerful, deadly laser!", "¡Pew! ¡Pew! ¡Dispara un láser poderoso y letal!" },
            { "a basic spell to get you started with magic.", "Un hechizo básico para empezar con la magia." },
            { "a more advanced spell. shockingly effective!", "Un hechizo más avanzado. ¡Sorprendentemente eficaz!" },
            { "a metallic tool. seems useless, but you do not\nwant your face smashed by a hammer...",
                "Una herramienta metálica. Parece inútil, pero no quieres tu cara\ndestrozada por un martillo..." },
            { "this is a fiery rock. no one knows where they\ncome from; luckily, you can make your own!",
                "Esta es una roca ardiente. Nadie sabe de dónde vienen;\nafortunadamente, ¡usted puede hacer su propio!" },
            { "the most basic type of rocket.", "El tipo más básico de cohete." },
            { "explodes and sets aliens on fire.", "Explota y establece los extraterrestres sobre el fuego." },
            { "explodes and poisons aliens.", "Explota y envenena a los extraterrestres." },
            { "explodes and freezes aliens.", "Explota y congela a los extraterrestres." },
            { "explodes and... plasma-fies aliens?", "Explota y... ¿plasma-fies los extraterrestres?" },
            { "explodes and freezes and burns and poisons and\nplasma-fies aliens.",
                "Explota y se congela y quemaduras y venenos y plasma-fies\nextraterrestres." },
            { "lump of ice that freezes aliens. they don't like\nsnowball fights that much.",
                "Un trozo de hielo que congela a los extraterrestres. No les gusta la\npelea de bolas de nieve." },
            { "don't know where this came from... and i don't\nthink you want to know...",
                "No sé de dónde vino esto... y no creo que quieras saber..." },
            { "a ninja's chosen weapon.", "El arma escogida de un ninja." },
            { "ouch! an ice cold shard right to the face.", "¡Ay! Un fragmento de hielo en la cara." },
            { "A more powerful spell that heals you and\nhurts your enemies.",
                "Un hechizo más poderoso que te cura y\nhiere a tus enemigos" },

            { "adds a faint bar for easier aiming", "Añade una barra débil para facilitar el apuntamiento" },
            { "adds an amount of damage to a projectile", "Añade una cantidad de daño a un proyectil" },
            { "your maximum health", "Tu salud máxima" },
            { "how fast the cannon can move", "La velocidad que el cañón se mueve" },
            { "allows for another projectile to fire immediately if holding down\nthe fire key",
                "Permite que otro proyectil dispare inmediatamente si\nmantiene pulsada la tecla de disparo" },
            { "the speed at which a new projectile is ready to fire",
                "La velocidad a la que un nuevo proyectil está listo para\ndisparar" },

            #endregion

            #region Tutorial Translation

            { "(press space to continue)", "(Pulse espacio para continuar)" },
            { "welcome to one cannon, one army!", "¡Bienvenido a One Cannon, One Army!" },
            { "i will be your guide.", "Seré tu guía." },
            { "let's get started!", "¡Empecemos!" },
            { "this is the start button.\nuse it to begin playing.",
                "Este es el botón de comenzar.\nUtilícelo para comenzar a jugar." },
            { "and here's the quit button.\nuse it to exit the game.",
                "Y aquí está el botón de salir.\nUsarlo para salir del juego." },
            { "this thing is the controls button.\nin this menu, you can customize the game's controls.",
                "Esta cosa es el botón controles.\nEn este menú, puede personalizar los controles del juego." },
            { "this is the language button.\nhere, you can change the game's language.",
                "Este es el botón de idioma.\nAquí puedes cambiar el idioma del juego." },
            { "now click on the start button to begin!", "¡Ahora haga clic en el botón comenzar para comenzar!" },
            { "here, you can view your users and create new ones.", "Aquí puedes ver a tus usuarios y crear otros nuevos." },
            { "click \"create new user\" to do so!", "¡Haga clic en \"Crea nuevo usuario\" para hacerlo!" },
            { "on this menu, you can create a new user.", "En este menú, puede crear un nuevo usuario." },
            { "type your username here.", "Escriba su nombre de usuario aquí." },
            { "you can change it later if needed.", "Puede cambiarlo más tarde si es necesario." },
            { "use these sliders to change the color of your avatar.",
                "Utilice estos deslizadores para cambiar el color de su avatar." },
            { "this is your icon.", "Este es tu ícono." },
            { "you can set your icon to any projectile by clicking the arrows below.",
                "Puede ajustar el icono a cualquier proyectil haciendo clic en las flechas de abajo." },
            { "your icon does not affect the game; it is just cosmetic.", "Su icono no afecta al juego; es sólo cosmético." },
            { "use the back button to cancel creating a user.", "Utilice el botón atrás para cancelar la creación de un usuario." },
            { "after you've customized your user, click \"create user\" to continue!",
                "Después de personalizar su usuario, ¡haga clic en \"Crea Usario\" \npara continuar!" },
            { "now, click on your username to sign in!", "Ahora, ¡haga clic en su nombre de usuario para iniciar sesión!" },
            { "now we're on the main menu.", "Ahora estamos en el menú principal." },
            { "this is the play button, where you begin your adventure!", "¡Este es el botón Juega, donde empiezas tu aventura!" },
            { "here's the achievement button, where you can view all achievements.",
                "Aquí está el botón de Logro, donde puedes ver todos los logros." },
            { "the stats button let's you view various statistics about you and your play.",
                "El botón Estadísticas vamos a ver varias estadísticas sobre usted y su juego." },
            { "the shop button is where you can purchase materials...", "El botón Tienda es donde se pueden comprar materiales..." },
            { "...and the crafting menu let's you make projectiles out of them!",
                "¡...y el menú de Elaboración vamos a hacer proyectiles de ellos!" },
            { "here you can upgrade your cannon.", "Aquí puede actualizar su cañón." },
            { "in the organize menu you can set your hotbar and view your inventory.",
                "En el menú organizar puede configurar su salpicadero y ver su inventario." },
            { "and in the gifts menu, you can view and open gifts.", "Y en el menú de Regalos, puedes ver y abrir regalos." },
            { "the options menu lets you change your name,\navatar, and icon (among other things).",
                "El menú de Opciones le permite cambiar su\nnombre, avatar y icono (entre otras cosas)." },
            { "now click \"play\" to begin a mission!", "¡Ahora haga clic en \"Juega\" para comenzar una misión!" },
            { "here, you can play missions.", "Aquí, puedes jugar a las misiones." },
            { "you cannot attempt a mission until you complete the previous one.",
                "No puedes intentar una misión hasta completar la anterior." },
            { "your goal is to beat level 25.", "Tu objetivo es vencer al nivel 25." },
            { "however, you can still replay previous missions if you desire.",
                "Sin embargo, usted todavía puede reproducir las misiones anteriores si desea." },
            { "now, click on mission \"0\" to try out the cannon!", "¡Ahora, haga clic en la misión \"0\" para probar el cañón!" },
            { "use esc. to pause the game.", "Utilice Esc. para pausar el juego." },
            { "the resume button lets you continue your game.", "El botón Continua le permite continuar su juego." },
            { "the main menu button returns you to the main menu.", "El botón Menú Principal le devuelve al menú principal." },
            { "the quit button will save your progress and close the game.",
                "El botón Deja guardará su progreso y cerrará el juego." },
            { "now click resume to play!", "¡Ahora haga clic en Continua para jugar!" },
            { "use the space bar to shoot a rock, and a and d to move around.\nkill the aliens to beat the level!",
                "Utilizar el botón espacio para disparar una roca, y A y D para moverse.\n¡Mata a los " +
                "extraterrestres para vencer al nivel!" },
            { "now press space to finish the\n tutorial and battle the aliens! good luck!",
                "¡Ahora presione Espacio para terminar\nel tutorial y luchar contra los extraterrestres! ¡Buena suerte!" },
            { "your user", "tu usario" },
            { "the escape (esc.) key", "la tecla Escape" },
            { "now let's make some rocks. to begin, open the shop.",
                "Ahora vamos a hacer unas rocas. Para empezar, abre la tienda." },
            { "here in the shop, you can buy one of the eight different materials.",
                "Aquí en la tienda, puedes comprar uno de los ocho materiales diferentes." },
            { "click on \"Buy 10\" under stone to buy ten stones.",
                "Haga clic en \"Compra 10\" debajo de la piedra para comprar diez piedras." },
            { "good job! now it's time to make some rocks and stop some evil aliens!",
                "¡Buen trabajo! ¡Ahora es el momento de hacer algunas rocas y detener los malvados extraterrestres!" },
            { "click on \"crafting\" to make some rocks.", "Haga clic en \"Elaboración\" para hacer algunas rocas." },
            { "now click \"craft\" under rock to use your stone to make a rock!",
                "¡Ahora haga clic en \"Crea\" bajo roca para utilizar su piedra para hacer una roca!" },
            { "great job! you've already been given 50 rocks to start you off, so let's get moving!",
                "¡Buen trabajo! ¡Ya te han dado 50 piedras para empezar, así que vamos a movernos!" },
            { "now let's go back to the main menu to fight some aliens!",
                "¡Ahora vamos a volver al menú principal para luchar contra algunos extraterrestres!" },

            #endregion

            #region Story Translation

            // STORY 1
            {  "it is a dark time on lithios.", "Es un tiempo oscuro en Lithios. " },
            { "resources are low, farms are failing, and lithians are dying.",
                "Recursos son bajos, las granjas están fallando y Lithians mueren." },
            { "a dangerous disease runs rampant. death's dark grip is closing on the planet.",
                "A corre desenfrenada. El cierre de agarre oscuro de la muerte en el planeta." },
            { "the lithian leader, malos, sees no other option; they must colonize another planet.",
                "El Lithian líder, Malos, no hay otra opción: debe colonizar otro planeta. " },
            { "with an army of his strongest warriors, malos heads out",
                "Con un ejército de sus guerreros más fuertes, Malos jefes a" },
            { "for a distant spiral galaxy. upon reaching it, they find an inhabitable planet.",
                "de una galaxia espiral lejana. Llegar, encuentran un planeta habitable." },
            { "the only problem: that planet is earth, and it is already inhabited.",
                "El único problema: que es la planeta tierra, y ya está habitada." },
            { "fortunately, the solar system is rich with", "Afortunadamente, el sistema solar es rico con" },
            { "planets and moons that can support the lithians.", "planetas y lunas que pueden soportar el Lithians." },
            { "unfortunately, the earthlings will likely defend their solar system with their lives.",
                "Por desgracia, los terrícolas defenderá probablemente su sistema solar con sus vidas. " },
            { "malos devises a clever plan: destroy earth.", "Malos idea un ingenioso plan: destruir la tierra." },
            { "after sending the message back home, construction", "Después de enviar el mensaje de vuelta a casa, empieza la construcción" },
            { "begins on three deadly weapons on lithios.", "en tres armas letales en Lithios." },
            { "weapons with the power to ensure the lithians' security in their new home.",
                "Armas con la potencia para garantizar la seguridad de la Lithians en su nuevo hogar." },
            { "weapons with the power to destroy a planet.", "Armas con el poder de destruir un planeta." },
            { "however, one small, insignificant lithian sees no purpose in destroying earth.",
                "Sin embargo, una pequeña, insignificante Lithian ve ningún propósito en la destrucción de la tierra." },
            { "there was never an attempt to befriend the earthlings;", "Que nunca fue un intento de hacerse amigo de los terrícolas;" },
            { "destroying them is ending more unnecessary lives, he thinks.",
                "destrucción de ellos está terminando más vidas innecesarios, él piensa." },
            { "so the alien, named tucker, heads for earth to", "Para las cabezas de alien, llamado Tucker, de la tierra para" },
            { "assist the small planet in its defenses.", "el planeta pequeño en sus defensas." },
            { "malos begins his attack. all is dark, hopeless.", "Malos comienza su ataque. Todo es oscuro, sin esperanza." },
            { "only a single person notices.", " Avisos de solamente una sola persona." },
            { "with a science experiment cannon and some small rocks,", "Con un cañón de experimento de la ciencia y algunas pequeñas rocas," },
            { "they alone must save the world.", "que sólo deben salvar el mundo." },
            { "alone, with no help.", "Solo, con no ayuda." },
            { "or the lithians will win.", "o el Lithians va a ganar." },
            { "the planet will be destroyed.", "El planeta será destruido." },
            { "and humans will be exterminated.", "y los seres humanos serán exterminados." },
            { "forever.", "Para siempre." },

            // STORY 2
            { "it's over.", "Se acabó." },
            { "malos has been defeated.", "Malos ha sido derrotado." },
            { "his lasers have been destroyed.", "Sus láseres han sido destruido." },
            { "his army has scattered.", "Su ejército ha dispersado." },
            { "but what of the others of lithios?", "Pero ¿qué pasa con los otros de Lithios?" },
            { "what of those malos tried so hard to save?", "¿Qué pasa con esos Malos intentado tan duro guardar?" },
            { "eventually, a they found a home.", "Eventualmente, encontraron un hogar." },
            { "a small planet-sized moon they dubbed lithios ii.", "Una pequeña planeta tamaño luna llamó Lithios II." },
            { "however, others might know it by its more common name:", "Sin embargo, otros pueden conocer por su nombre más común:" },
            { "europa, a moon of jupiter.", "Europa, una luna de Júpiter." },
            { "and the lithians finally proved that they can live in peace", "Y los Lithians finalmente demostró que puede vivir en paz" },
            { "with humans.", "con los seres humanos." },
            { "and still today, if you look up in the sky,", "Y aún hoy, si miras para arriba en el cielo," },
            { "you may see a lithian ship taking flight.", "puede ver un Lithian nave toma vuelo." },
            { "they wave to us. they thank us.", "Ellos onda a nosotros. Ellos nos lo agradecerán." },
            { "but really, we should be thanking them.", "Pero realmente, nosotros debemos ser agradeciéndoles." },

            #endregion

            #region Subtitle Translations

            { "purasu intro", "Introducción de Purasu" },
            { "spell", "Hechizo" },
            { "doors open", "Puertas abren" },
            { "doors close", "Puertas cierran" },
            { "item crafted", "Artículo elabora" },
            { "cannon fire", "Estruendo de cañón" },
            { "cannon clunk", "Sonido de cañón" },
            { "explosion", "Explosión" },
            { "zap", "Zas" },
            { "click", "Clic" },
            { "firework", "Pirotécnico" },
            { "life earned", "Vida ganada" },
            { "achievement", "Logro" },
            { "alien death", "Muerte de extraterrestre" },
            { "coin", "Moneda" },
            { "success", "Éxito" },
            { "failure", "Fracaso" },
            { "shield break", "Ruptura del escudo" },
            { "alien hit", "Golpe de extraterrestre" },
            { "laser charge", "Láser cargo" },
            { "metal shaking", "Temblor de metal" },
            { "player hit", "Golpe de jugador" },
            { "notification", "Notificación" },
            { "material found", "Material encontrado" },
            { "projectile found", "Proyectil encontrado" },
            { "cage hit", "Golpe de jaula" },
            // "Upgrade", "Sweep", and "Laser" are already defined

            #endregion
        };
        #endregion
        #region French
        static Dictionary<string, string> frenchDict = new Dictionary<string, string>()
        {

        };
        #endregion
        #region Esperanto
        static Dictionary<string, string> esperantoDict = new Dictionary<string, string>()
        {

        };
        #endregion
        #region Italian
        static Dictionary<string, string> italianDict = new Dictionary<string, string>()
        {

        };
        #endregion
        #region German
        static Dictionary<string, string> germanDict = new Dictionary<string, string>()
        {

        };
        #endregion
        #region UK English
        static Dictionary<string, string> enUkDict = new Dictionary<string, string>()
        {

        };
        #endregion
        #region Tagalog
        static Dictionary<string, string> tagalogDict = new Dictionary<string, string>()
        {

        };
        #endregion

        public static string Translate(string text)
        {
            return TranslateInto(text, GameInfo.Language);
        }
        public static string TranslateInto(string text, Language language)
        {
            switch (language)
            {
                case Language.English:
                    return text;
                case Language.Spanish:
                    if (spanishDict.ContainsKey(text.ToLower()))
                    {
                        return spanishDict[text.ToLower()];
                    }
                    break;
                //case Language.French:
                //    if (frenchDict.ContainsKey(text.ToLower()))
                //    {
                //        return frenchDict[text.ToLower()];
                //    }
                //    break;
                //case Language.Esperanto:
                //    if (esperantoDict.ContainsKey(text.ToLower()))
                //    {
                //        return esperantoDict[text.ToLower()];
                //    }
                //    break;
                //case Language.Italian:
                //    if (italianDict.ContainsKey(text.ToLower()))
                //    {
                //        return italianDict[text.ToLower()];
                //    }
                //    break;
                //case Language.German:
                //    if (germanDict.ContainsKey(text.ToLower()))
                //    {
                //        return germanDict[text.ToLower()];
                //    }
                //    break;
                //case Language.EnglishUK:
                //    if (enUkDict.ContainsKey(text.ToLower()))
                //    {
                //        return enUkDict[text.ToLower()];
                //    }
                //    break;
                //case Language.Tagalog:
                //    if (tagalogDict.ContainsKey(text.ToLower()))
                //    {
                //        return tagalogDict[text.ToLower()];
                //    }
                //    break;
            }
            return text;
        }
    }

    public enum Language
    {
        English, // EN(US) - English (US)
        Spanish, // ES(MX) - Español (MX)
        //French, // FR - Français
        //Esperanto, // EO - Esperanto
        //Italian, // IT - Italiano
        //German, // DE - Deutsche
        //EnglishUK, // EN(UK) - English (UK)
        //Tagalog, // TL - Tagalog
    }

    public class LanguageSelectMenu
    {
        #region Fields

        MenuButton enButton;
        MenuButton esButton;
        //MenuButton frButton;
        //MenuButton eoButton;
        //MenuButton itButton;
        //MenuButton deButton;
        //MenuButton ukButton;
        //MenuButton tlButton;

        const int Y_OFFSET = 150;
        const int SPACING = 5;

        #endregion

        #region Constructors

        public LanguageSelectMenu(int windowWidth, int windowHeight, GraphicsDevice graphics, SpriteFont font,
            Action<Language> changeLang)
        {
            int lastY = Y_OFFSET;

            enButton = new MenuButton(new System.Action(() => changeLang(Language.English)), "English (US)", 0, 0,
                true, font, graphics);
            enButton.X = windowWidth / 2 - (enButton.Width / 2);
            enButton.Y = lastY;
            lastY += enButton.Height + SPACING;

            esButton = new MenuButton(new System.Action(() => changeLang(Language.Spanish)), "Español (MX)", 0, 0,
                true, font, graphics);
            esButton.X = windowWidth / 2 - (esButton.Width / 2);
            esButton.Y = lastY;
            lastY += esButton.Height + SPACING;

            //frButton = new MenuButton(new Action(() => changeLang(Language.French)), "Français", 0, 0,
            //    true, font, graphics);
            //frButton.X = windowWidth / 2 - (frButton.Width / 2);
            //frButton.Y = lastY;
            //lastY += frButton.Height + SPACING;

            //deButton = new MenuButton(new Action(() => changeLang(Language.German)), "Deutsche", 0, 0,
            //    true, font, graphics);
            //deButton.X = windowWidth / 2 - (deButton.Width / 2);
            //deButton.Y = lastY;
            //lastY += deButton.Height + SPACING;

            //eoButton = new MenuButton(new Action(() => changeLang(Language.Esperanto)), "Esperanto", 0, 0,
            //    true, font, graphics);
            //eoButton.X = windowWidth / 2 - (eoButton.Width / 2);
            //eoButton.Y = lastY;
            //lastY += eoButton.Height + SPACING;

            //itButton = new MenuButton(new Action(() => changeLang(Language.Italian)), "Italiano", 0, 0,
            //    true, font, graphics);
            //itButton.X = windowWidth / 2 - (itButton.Width / 2);
            //itButton.Y = lastY;
            //lastY += itButton.Height + SPACING;

            //ukButton = new MenuButton(new Action(() => changeLang(Language.EnglishUK)), "English (UK)", 0, 0,
            //    true, font, graphics);
            //ukButton.X = windowWidth / 2 - (ukButton.Width / 2);
            //ukButton.Y = lastY;
            //lastY += ukButton.Height + SPACING;

            //tlButton = new MenuButton(new Action(() => changeLang(Language.Tagalog)), "Tagalog (Filipino)", 0, 0,
            //    true, font, graphics);
            //tlButton.X = windowWidth / 2 - (tlButton.Width / 2);
            //tlButton.Y = lastY;
        }

        #endregion

        #region Public Methods

        public void Update()
        {
            enButton.Active = GameInfo.Language != Language.English;
            enButton.Update();
            esButton.Active = GameInfo.Language != Language.Spanish;
            esButton.Update();
            //frButton.Update();
            //itButton.Update();
            //eoButton.Update();
            //deButton.Update();
            //ukButton.Update();
            //tlButton.Update();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            enButton.Draw(spriteBatch);
            esButton.Draw(spriteBatch);
            //frButton.Draw(spriteBatch);
            //itButton.Draw(spriteBatch);
            //eoButton.Draw(spriteBatch);
            //deButton.Draw(spriteBatch);
            //ukButton.Draw(spriteBatch);
            //tlButton.Draw(spriteBatch);
        }

        public List<MenuButton> GetButtons()
        {
            List<MenuButton> returnVal = new List<MenuButton>()
            {
                enButton,
                esButton,
                //frButton,
                //itButton,
                //eoButton,
                //deButton,
                //ukButton,
                //tlButton,
            };
            return returnVal;
        }

        #endregion
    }
}