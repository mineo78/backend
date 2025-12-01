const connection = new signalR.HubConnectionBuilder()
    .withUrl("/gameHub")
    .build();

connection.start().catch(err => console.error(err.toString()));

const gameBoard = document.getElementById("gameBoard");

function renderBoard(board) {
    gameBoard.innerHTML = '';
    board.forEach((row, rowIndex) => {
        const tr = document.createElement("tr");
        row.forEach((cell, colIndex) => {
            const td = document.createElement("td");
            td.textContent = cell === 1 ? '🔴' : cell === 2 ? '🟡' : '';
            td.style.width = '50px';
            td.style.height = '50px';
            td.style.textAlign = 'center';
            td.style.verticalAlign = 'middle';
            td.style.cursor = 'pointer';
            td.style.backgroundColor = '#f0f0f0';

            // Si la case est vide, permet de cliquer pour faire un coup
            if (cell === 0) {
                td.addEventListener("click", () => {
                    connection.invoke("MakeMove", rowIndex, colIndex).catch(err => console.error(err.toString()));
                });
            }

            tr.appendChild(td);
        });
        gameBoard.appendChild(tr);
    });
}

function renderLobbyList(lobbies) {
    const list = document.getElementById("lobbyList");
    list.innerHTML = ''; // Réinitialise la liste des salons

    lobbies.forEach(lobby => {
        if (!lobby.Players) {
            console.error("Lobby sans joueurs détecté :", lobby);
            return;
        }

        const li = document.createElement("li");
        li.textContent = `${lobby.Name} (${lobby.Players.length}/2 joueurs)`;

        // Si le lobby n'est pas encore commencé et il y a de la place
        if (!lobby.IsStarted && lobby.Players.length < 2) {
            const joinButton = document.createElement("button");
            joinButton.textContent = "Rejoindre";
            joinButton.onclick = () => {
                const playerName = document.getElementById("playerName").value;
                connection.invoke("JoinLobby", lobby.Name, playerName)
                    .catch(err => console.error(err.toString()));
            };
            li.appendChild(joinButton);
        }

        list.appendChild(li);
    });
}

// Mise à jour du tableau de jeu avec l'état actuel
connection.on("UpdateBoard", board => {
    renderBoard(board);
});

// Mise à jour de la liste des salons de jeu
connection.on("UpdateLobbyList", lobbies => {
    console.log("Lobbys mis à jour :", lobbies);
    renderLobbyList(lobbies);
});

// Lorsque la partie est terminée
connection.on("GameOver", winner => {
    alert(`Le joueur ${winner === 1 ? "🔴" : "🟡"} a gagné !`);
    connection.invoke("MakeMove", -1, -1); // Recharge le tableau après la victoire
});

// Initialisation du tableau de jeu
connection.on("InitBoard", board => {
    renderBoard(board);
});

// Lorsque le jeu commence
connection.on("GameReady", (lobbyName, gameType, gameData) => {
    if (gameType === "Puissance4") {
        window.location.href = `/Home/Puissance4?lobby=${lobbyName}`;
    }
});
