'use strict';

// Global state. Muahahaha.
let height = 40;
let width = 80;
let cellSize = 20;
let liveCells = new Map();
let playing = false;
let mouseDown = false;

/**
 * Translate the global state into a state object to send to the server
 */
function getState() {
    const liveCellList = [];
    for (const [row, activeCols] of liveCells.entries()) {
        for (const col of activeCols) {
            liveCellList.push({ row, col });
        }
    }
    return { height, width, liveCells: liveCellList };
}

/**
 * Update the global state based on a state object from the server
 */
function setState(state) {
    height = state.height;
    width = state.width;
    liveCells = new Map();
    for (const coords of state.liveCells) {
        const i = coords.row;
        const j = coords.col;
        if (!liveCells.has(i)) {
            liveCells.set(i, new Set());
        }
        liveCells.get(i).add(j);
    }
    render();
}

/**
 * Updates the global state so that the cell at position (i,j) is either active or not
 */
function setCellActive(i, j, isActive) {
    if (!liveCells.has(i) && isActive) {
        liveCells.set(i, new Set([j]));
    } else if (liveCells.get(i).has(j) && !isActive) {
        liveCells.get(i).delete(j);
    } else if (isActive) {
        liveCells.get(i).add(j);
    }
}

/**
 * Queries the global state to see if the cell at (i,j) is active
 */
function isCellActive(i, j) {
    return liveCells.has(i) && liveCells.get(i).has(j);
}

/**
 * Get an initial state object from the server, set up the DOM, and set up the initial state
 */
async function init() {
    const res = await fetch(`/GameOfLife?width=${width}&height=${height}`);
    drawBoard();
    addGlobalListeners();
    setState(await res.json());
}

/**
 * Set up the DOM. This should only need to be called once.
 */
function drawBoard() {
    const board = document.querySelector('.board');
    board.innerHTML = '';

    board.style.gridTemplateRows = `repeat(${height}, ${cellSize}px)`;
    board.style.gridTemplateColumns = `repeat(${width}, ${cellSize}px)`;

    for (let i = 0; i < height; i++) {
        for (let j = 0; j < width; j++) {
            const cell = document.createElement('div');
            cell.classList.add('cell');
            cell.setAttribute('data-row-index', i);
            cell.setAttribute('data-col-index', j);
            cell.addEventListener('click', onCellClick);
            cell.addEventListener('mouseenter', onCellMouseEnter)
            board.appendChild(cell);
        }
    }
}

/**
 * Updates the DOM based on the global state.
 */
function render() {
    // Clear active cells
    for (const activeCell of document.querySelectorAll('.cell.live')) {
        activeCell.classList.remove('live');
    }
    for (const [i, activeCols] of liveCells.entries()) {
        for (const j of activeCols) {
            const selector = `[data-row-index="${i}"][data-col-index="${j}"]`;
            const cell = document.querySelector(selector);
            cell.classList.add('live');
        }
    }

    if (playing) {
        document.querySelector('.clear-button').setAttribute('disabled', 'disabled');
        document.querySelector('.iterate-button').setAttribute('disabled', 'disabled');
        document.querySelector('.play-pause-button').innerHTML = 'Pause';
    } else {
        document.querySelector('.clear-button').removeAttribute('disabled');
        document.querySelector('.iterate-button').removeAttribute('disabled');
        document.querySelector('.play-pause-button').innerHTML = 'Play';
    }
}

/**
 * Send the current state to the server, and update with the next iteration
 */
async function iterate() {
    const method = 'POST';
    const body = JSON.stringify(getState());
    const headers = new Headers();
    headers.set('Content-Type', 'application/json');

    const res = await fetch('/GameOfLife/Iterate', { method, body, headers });
    setState(await res.json());
}

/**
 * Clicking the play button causes the game to proceed or pause
 */
function togglePlay() {
    playing = !playing;
    if (playing) {
        play();
    }
}

/**
 * Update the state until `playing` is false;
 */
async function play() {
    if (!playing) return Promise.resolve();
    await iterate();
    await play();
}

/**
 * Clicking the clear button does what you would expect.
 */
function clearBoard() {
    liveCells = new Map();
    render();
}

/**
 * If a dead cell is clicked, make it alive.
 * If a live cell is clicked, make it dead.
 */
function onCellClick(e) {
    if (!e.target.classList.contains('cell')) return;

    const i = parseInt(e.target.getAttribute('data-row-index'));
    const j = parseInt(e.target.getAttribute('data-col-index'));

    setCellActive(i, j, !isCellActive(i, j));
    render();
}

/**
 * Adds global event listeners to track whether the mouse is held down.
 * This is so that we can activate many cells by clicking and dragging.
 */
function addGlobalListeners() {
    window.addEventListener('mousedown', () => mouseDown = true);
    window.addEventListener('mouseup', () => mouseDown = false);
}

/**
 * If the mouse is down when hovering over a cell, activate it
 */
function onCellMouseEnter(e) {
    if (!e.target.classList.contains('cell')) return;
    if (!mouseDown) return;

    const i = parseInt(e.target.getAttribute('data-row-index'));
    const j = parseInt(e.target.getAttribute('data-col-index'));

    setCellActive(i, j, true);
    render();
}
