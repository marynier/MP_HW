import { Room, Client } from "colyseus";
import { Schema, type, MapSchema } from "@colyseus/schema";

// const spawnPoints = [
//     { x: -20, y: 0, z: -20 },
//     { x: 20, y: 0, z: 20 },
//     { x: -20, y: 0, z: 20 },
//     { x: 20, y: 0, z: -20 },
//     { x: 10, y: 0, z: -10 },
//     { x: -10, y: 0, z: 10 },
//     { x: -10, y: 0, z: -10 },
//     { x: 10, y: 0, z: -10 },

// ];

export class Player extends Schema {
    @type("uint8")
    skin = 0;

    @type("number")
    gun = 1;    

    @type("uint8")
    loss = 0;

    @type("int8")
    maxHP = 0;

    @type("int8")
    currentHP = 0;

    @type("boolean")
    crouch = false;   

    @type("number")
    speed = 0;    

    @type("number")
    pX = 0;

    @type("number")
    pY = 0;

    @type("number")
    pZ = 0;

    @type("number")
    vX = 0;

    @type("number")
    vY = 0;

    @type("number")
    vZ = 0;    

    @type("number")
    rX = 0;

    @type("number")
    rY = 0;  
}

export class State extends Schema {
    @type({ map: Player })
    players = new MapSchema<Player>();

    something = "This attribute won't be sent to the client-side";

    @type(["number"])
    spawnPoints: {x: number; y: number; z: number}[] = [];
    freeSpawnPoints: {x: number; y: number; z: number}[] = [];
    playerSpawnPoints = new MapSchema<{x: number; y: number; z: number}>();

    initSpawnPoints(points: {x: number; y: number; z: number}[]) {
        this.spawnPoints = points;
        this.freeSpawnPoints = [...points];
    }

    getSpawnPoint(): { x: number; y: number; z: number } | null {
        if (this.freeSpawnPoints.length === 0) return null;
        const idx = Math.floor(Math.random() * this.freeSpawnPoints.length);
        const point = this.freeSpawnPoints[idx];
        this.freeSpawnPoints.splice(idx, 1);
        return point;
    }

    releaseSpawnPoint(point: {x: number; y: number; z: number}) {
        this.freeSpawnPoints.push(point);
    }


    //freeSpawnPoints: { x: number; y: number; z: number }[] = [...spawnPoints];

    // getSpawnPoint(): { x: number; y: number; z: number } | null {
    //     if (this.freeSpawnPoints.length === 0) return null;
    //     const index = Math.floor(Math.random() * this.freeSpawnPoints.length);
    //     const point = this.freeSpawnPoints[index];
    //     this.freeSpawnPoints.splice(index, 1); //удаление точки из свободных
    //     return point;
    // }

    // releaseSpawnPoint(point: {x: number; y: number; z: number}) {
    //     this.freeSpawnPoints.push(point); //возвращение точки в массив
    // }

    //playerSpawnPoints = new MapSchema<{x: number; y: number; z: number}>(); //структура для хранения текущей спавн-точки игрока

    createPlayer(sessionId: string, data: any, skin: number) {
        //const spawnPoint = this.getSpawnPoint() || { x: 0, y: 0, z: 0 };

        const player = new Player();
        player.skin = skin;
        player.maxHP = data.hp;
        player.currentHP = data.hp;
        player.speed = data.speed;
        player.pX = data.pX;
        player.pY = data.pY;
        player.pZ = data.pZ;
        player.rY = data.rY;

        //использование выделенной на сервере позиции
        //player.pX = spawnPoint.x;
        //player.pY = spawnPoint.y;
        //player.pZ = spawnPoint.z;

        this.players.set(sessionId, player);
        //this.playerSpawnPoints.set(sessionId, spawnPoint);
    }

    removePlayer(sessionId: string) {
        const player = this.players.get(sessionId);
        if(player) {
            //возвращение точки спавна в массив
            //this.releaseSpawnPoint({x: player.pX, y: player.pY, z: player.pZ});
            this.players.delete(sessionId);
        }        
    }

    movePlayer (sessionId: string, data: any) {
        const player = this.players.get(sessionId);
        //this.releaseSpawnPoint(this.playerSpawnPoints.get(sessionId)); //возвращение точки спавна в массив
        player.pX = data.pX;
        player.pY = data.pY;
        player.pZ = data.pZ;
        player.vX = data.vX;
        player.vY = data.vY;
        player.vZ = data.vZ;        
        player.rX = data.rX;
        player.rY = data.rY;
        
        const spawn = this.playerSpawnPoints.get(sessionId);
        if (spawn) this.releaseSpawnPoint(spawn);
                        
    }
    crouchPlayer (sessionId: string, data: any) {
        const player = this.players.get(sessionId);
        player.crouch = data.crouch;
    }
    
    changeGun (sessionId: string, data: any) {
        const player = this.players.get(sessionId);
        player.gun = data.gun;     
    } 
}

export class StateHandlerRoom extends Room<State> {
    maxClients = 2;
    spawnPointCount = 1;
    skins: number[] = [0];

    mixArray(arr){
        var currentIndex = arr.length;
        var tmpValue, randomIndex;
        while (currentIndex !== 0){
            randomIndex = Math.floor(Math.random() * currentIndex);
            currentIndex -= 1;
            tmpValue = arr[currentIndex];
            arr[currentIndex] = arr[randomIndex];
            arr[randomIndex] = tmpValue;
        }
    }

    onCreate (options) {
        for(var i = 1; i < options.skins; i++ ){
            this.skins.push(i);  
        }
        this.mixArray(this.skins);

        this.spawnPointCount = options.points;

        console.log("StateHandlerRoom created!", options);

        this.setState(new State());
        //this.setPatchRate(50);
        this.state.initSpawnPoints(options.spawnPoints);

        this.onMessage("move", (client, data) => {
            //console.log("StateHandlerRoom received message from", client.sessionId, ":", data);
            this.state.movePlayer(client.sessionId, data);
        });

        this.onMessage("crouch", (client, data) => {           
            this.state.crouchPlayer(client.sessionId, data);            
        });

        this.onMessage("gunChange", (client, data) => {           
            this.state.changeGun(client.sessionId, data);                        
        });

        this.onMessage("shoot", (client, data) => {
            this.broadcast("Shoot", data, {except: client})
        });
        
        this.onMessage("damage", (client, data) => {
            const clientID = data.id;
            const player = this.state.players.get(clientID);
            let hp = player.currentHP - data.value;
            if(hp > 0){
                player.currentHP = hp;
                return;
            }

            player.loss++;
            player.currentHP = player.maxHP;

            for(var i=0; i < this.clients.length; i++){
                if(this.clients[i].id != clientID) continue;
                const point = Math.floor(Math.random() * this.spawnPointCount);
                
                this.clients[i].send("Restart", point);
            }  

            //если точка спавна все еще зарезервирована умершим, возвращаем ее в массив
            //const oldSpawn = this.state.playerSpawnPoints.get(data.id);
            //if(oldSpawn) {
            //this.state.releaseSpawnPoint(oldSpawn);
            //}            
 
            //выделяем новую точку для возрождения
            //const newSpawn = this.state.getSpawnPoint() || {x:0, y:0, z:0};
            //this.state.playerSpawnPoints.set(data.id, newSpawn);

            //const message = JSON.stringify({ x: newSpawn.x, z: newSpawn.z });
            //this.clients[i].send("Restart", message);
            //}           
        })
    }

    onAuth(client, options, req) {
        return true;
    }

    onJoin (client: Client, options: any) {
        if(this.clients.length > 1) this.lock();
        const spawn = this.state.getSpawnPoint() || {x:0, y:0, z:0};
        this.state.playerSpawnPoints.set(client.sessionId, spawn); //привязать точку к игроку
        const skin = this.skins[this.clients.length -1];
        this.state.createPlayer(client.sessionId, {
        ...options,
        pX: spawn.x,
        pY: spawn.y,
        pZ: spawn.z,
        }, skin);
    }

    onLeave (client) {
        const spawn = this.state.playerSpawnPoints.get(client.sessionId);
        if (spawn) {
        this.state.releaseSpawnPoint(spawn);
        this.state.playerSpawnPoints.delete(client.sessionId);
        }

        this.state.removePlayer(client.sessionId);
    }

    onDispose () {
        console.log("Dispose StateHandlerRoom");
    }
}
