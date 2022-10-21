import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'monopoly-kata';
  private readonly MIN_PLAYER_COUNT = 2;
  private readonly MAX_PLAYER_COUNT = 8;
  private readonly MIN_DICE_ROLL_VAL = 2;
  private readonly MAX_ROUNDS = 20;
  private availableNames = [ "Hat", "Thimble", "Boot", "Dog", "Ship", "Iron", "Car", "Wheelbarrow" ]
  private currentPlayer = 0;
  public readonly boardLocations = new Array<any>(40);
  public players: Player[] = [];
  
  private _roundCount : number = 1;
  public get roundCount() : number {
    return this._roundCount;
  }
 
  public start() {
    if(this.players.length < this.MIN_PLAYER_COUNT || 
      this.players.length > this.MAX_PLAYER_COUNT){
      throw new Error()
    }

    this.players = this.randomArrayShuffle(this.players);
  }

  public newGame(players: number){
    for (let index = 0; index < players; index++) {
      this.players.push(new Player(this.availableNames[index]));
    }
  }

  public rollDice() {
    return Math.floor((Math.random() * 10) + this.MIN_DICE_ROLL_VAL);
  }

  public moveCurrentPlayer(rollNumber: number){
    this.checkRoundValidity();

    let playerLocation = this.players[this.currentPlayer].location;

    playerLocation += rollNumber;

    this.players[this.currentPlayer].location = this.exceedsBoardPositions(playerLocation) ? 
      (playerLocation -= this.boardLocations.length) : playerLocation;

    this.endPlayerTurn();
  }

  private endPlayerTurn() {
    this.players[this.currentPlayer].roundCount++;
    this.currentPlayer++;
    if (this.isFinalPlayer()){
      this.startNewRound();
    }
  }

  private startNewRound() {
    this.currentPlayer = 0;
      this._roundCount++;
  }

  private checkRoundValidity() {
    if (this._roundCount > this.MAX_ROUNDS) 
      throw new Error("Game ended");
  }

  private isFinalPlayer = () => { return this.currentPlayer === this.players.length };

  private exceedsBoardPositions = (playerLocation: number) => { return playerLocation > this.boardLocations.length };

  randomArrayShuffle(array) {
    var currentIndex = array.length, temporaryValue, randomIndex;
    while (0 !== currentIndex) {
      randomIndex = Math.floor(Math.random() * currentIndex);
      currentIndex -= 1;
      temporaryValue = array[currentIndex];
      array[currentIndex] = array[randomIndex];
      array[randomIndex] = temporaryValue;
    }
    return array;
  }
}

export class Player {
  public name: string;
  public location: number;
  public roundCount: number;

  constructor(name: string) {
    this.name = name;
    this.location = 0;
    this.roundCount = 0;
  }
}
