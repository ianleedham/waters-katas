import { TestBed, async, ComponentFixture } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { AppComponent } from './app.component';

/*
Release 1: Basic Board and Player Movement

- All players start on the first location. DONE

- Between 2 and 8 named players.  DONE

- The players’ order is initially determined randomly DONE
  - and then maintained for the remainder of the game
  - verify that in every round the order of the players remained the same

- Each player takes a turn, roll a pair of dice, move number of places indicated on the dice DONE
  - Player on beginning location (numbered 0), rolls 7, ends up on location 7 
  - Player on location numbered 39, rolls 6, ends up on location 5

- The board has a total of 40 locations. DONE

- When the player reaches the end of the board, s/he starts back at the beginning again DONE

- Since this version is so simple, we’ll simply play a total of 20 rounds
  - verify that the total rounds was 20 DONE
  - and that each player played 20 rounds DONE

*/
describe('AppComponent', () => {
  let fixture: ComponentFixture<AppComponent>;
  let monopolyGame: AppComponent;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [
        RouterTestingModule
      ],
      declarations: [
        AppComponent
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(AppComponent);
    monopolyGame = fixture.debugElement.componentInstance;
  }));

  describe('2 Players', () => {
    beforeEach(() => monopolyGame.newGame(2));

    it('should have a board with 40 locations', () => {
      expect(monopolyGame.boardLocations.length).toEqual(40);
    });
  
    it('should have a minimum of two players', () => {
      expect(monopolyGame.players.length).toBeGreaterThan(1);
    });
  
    it('should have named players', () => {
      expect(monopolyGame.players[0].name).toEqual("Hat");
      expect(monopolyGame.players[1].name).toEqual("Thimble");
    });

    it('should allow the second player to roll after the first player finishes', () => {
      // Moves player 1
      monopolyGame.moveCurrentPlayer(4);
      // Should move player 2
      monopolyGame.moveCurrentPlayer(6);

      expect(monopolyGame.players[1].location).toEqual(6);
    });

    it('should allow the first player to roll after the last player finishes', () => {
      // Moves player 1
      monopolyGame.moveCurrentPlayer(4);
      // Should move player 2
      monopolyGame.moveCurrentPlayer(6);
      // Should move player 1 again
      monopolyGame.moveCurrentPlayer(3);

      expect(monopolyGame.players[0].location).toEqual(7);
    });

    it('should start on round one', () => {
      expect(monopolyGame.roundCount).toEqual(1);
    });

    it('should increment round after every player has had one turn', () => {
      
      for (let index = 0; index < monopolyGame.players.length; index++) {
        monopolyGame.moveCurrentPlayer(4);
      }   
      
      expect(monopolyGame.roundCount).toEqual(2);
    });

    it('should end game after round 20', () => {
      completeAllRounds();
            
      expect(() => monopolyGame.moveCurrentPlayer(1)).toThrowError();
      expect(monopolyGame.players[0].location).toEqual(20);
    });

    it('should allow all players to play only 20 rounds', () => {
      completeAllRounds();

      monopolyGame.players.forEach(player => {
        expect(player.roundCount).toEqual(20);
      });
    });

    function completeAllRounds(){
      const maxRoundCount = 20;
      for (let index = 0; index < maxRoundCount * monopolyGame.players.length; index++) {        
        monopolyGame.moveCurrentPlayer(1);
      }
    }
  });

  describe('8 Players', () => {
    beforeEach(() => monopolyGame.newGame(8));

    it('Can configure up to eight players', () => {
      expect(monopolyGame.players.length).toEqual(8);
    });

    it('all players start at first location', () => {
      monopolyGame.players.forEach(player => {
        expect(player.location).toEqual(0);
      })
    });

    it('should randomize the players order before starting the game', () => {
      let playersCopy = Object.assign([], monopolyGame.players);
  
      monopolyGame.start();
  
      expect(playersCopy).not.toEqual(monopolyGame.players);
    });

    it('should keep the player order at the start of new round', () => {
      monopolyGame.start();
      let playersCopy = Object.assign([], monopolyGame.players);

      for(let i = 0; i < monopolyGame.players.length; i++) {
        monopolyGame.moveCurrentPlayer(1);
      }
  
      expect(monopolyGame.roundCount).toEqual(2);
      expect(playersCopy).toEqual(monopolyGame.players);
    });
  
    [
      { rollNumber: 3 },
      { rollNumber: 6 }
    ].forEach(testCase => {
      it(`should move player as per dice: ${testCase.rollNumber}`, () => {
        monopolyGame.start();
        
        monopolyGame.moveCurrentPlayer(testCase.rollNumber);
    
        expect(monopolyGame.players[0].location).toEqual(testCase.rollNumber);
      });
    });

    it(`should move the first player to location 5 from location 39 when a 6 is rolled`, () => {
      monopolyGame.start();
      
      monopolyGame.players[0].location = 39;
      
      monopolyGame.moveCurrentPlayer(6);
  
      expect(monopolyGame.players[0].location).toEqual(5);
    });
  });

  describe('no setup', () => {
    [
      { numberOfPlayers: 1 },
      { numberOfPlayers: 9 }
    ].forEach((testCase) => {
      it(`should not allow the game to start with ${testCase.numberOfPlayers} player(s)`, () => {
        monopolyGame.newGame(testCase.numberOfPlayers);

        expect(() => { monopolyGame.start() }).toThrow();
      });
    });

    it('should return a number equal or greater than two when dice are rolled', () => {
      monopolyGame.newGame(2);
      monopolyGame.start();
      
      const roll = monopolyGame.rollDice();

      expect(roll).toBeGreaterThanOrEqual(2);
    });

    it('should return a number equal or less than 12 when dice are rolled', () => {
      monopolyGame.newGame(2);
      monopolyGame.start();
      
      const roll = monopolyGame.rollDice();

      expect(roll).toBeLessThanOrEqual(12);
    });

  });

});


