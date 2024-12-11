import { useEffect, useState } from "react";
import "./styles.css";
import "./styleBorder.css";

function Playground({ points, onRestart, onPause }) {
  let time = Math.round(Date.now() / 1000) - points.started - points.pauseTime;
  let scoreLeft = points.left;
  let scoreRight = points.right;
  return (
    <>
      <div
        style={{
          textAlign: "right",
          fontSize: "smaller",
        }}
      >
        Time: {time}s
      </div>
      <table className={"sasa"}>
        <tbody>
          <tr>
            <td className="top-left"></td>
            <td className="top-right"></td>
          </tr>
          <tr>
            <td className="left"></td>
            <td className="right"></td>
          </tr>
          <tr>
            <td className="left"></td>
            <td className="right"></td>
          </tr>
          <tr>
            <td className="bottom-left"></td>
            <td className="bottom-right"></td>
          </tr>
          <tr>
            <td style={{ textAlign: "left" }}>
              <button onClick={onPause}> Pause</button>
              <span style={{ float: "right", marginRight: "-3px" }}>
                {scoreLeft} :
              </span>
            </td>
            <td style={{ textAlign: "right" }}>
              <span style={{ float: "left", marginLeft: "1px" }}>
                &nbsp;{scoreRight}
              </span>
              <button onClick={onRestart}> Restart</button>
            </td>
          </tr>
        </tbody>
      </table>
    </>
  );
}

function Paddle(props) {
  let y = 50 + props.y;
  return (
    <div
      style={{
        position: "absolute",
        left: props.which == "left" ? 7 : 386,
        top: y,
        width: 7,
        height: 40,
        backgroundColor: props.which == "left" ? "#0000ff" : "#ff0000",
        borderRadius: "5px",
        border: "1px solid #ffffff",
      }}
    ></div>
  );
}

function Ball({ ballState }) {
  let x = ballState.x - 5;
  let y = ballState.y - 5;
  return (
    <div
      style={{
        position: "absolute",
        left: x,
        top: y,
        backgroundColor: "#ffffff",
        width: 10,
        height: 10,
        borderRadius: "5px",
      }}
    ></div>
  );
}

function generateVelocity() {
  let velocity;
  do {
    velocity = (Math.random() - 0.5) * 5;
  } while (Math.abs(velocity) < 1);
  return velocity;
}

function generateInitialBallState() {
  return {
    x: 200,
    y: 100,
    vx: generateVelocity(),
    vy: generateVelocity(),
    pause: false,
  };
}

function resetujPunkty() {
  return {
    left: 0,
    right: 0,
    started: Math.round(Date.now() / 1000),
    pauseTime: 0,
    pauseStarted: null,
  };
}

export default function App() {
  let [leftY, setLeftY] = useState(30);
  let [rightY, setRightY] = useState(30);
  let activeKeys = new Set();
  let [ballState, setBallState] = useState(generateInitialBallState());
  let [punkty, setPunkty] = useState(resetujPunkty());

  useEffect(() => {
    const handleKeyDown = (event) => {
      activeKeys.add(event.key);
    };

    const handleKeyUp = (event) => {
      activeKeys.delete(event.key);
    };

    const intervalId = setInterval(() => {
      if (ballState.pause) {
        return;
      }
      if (activeKeys.has("q")) {
        setLeftY((prevY) => Math.max(prevY - 1, -34));
      }
      if (activeKeys.has("a")) {
        setLeftY((prevY) => Math.min(prevY + 1, 40 * 3 + 9 - 34));
      }
      if (activeKeys.has("p")) {
        setRightY((prevY) => Math.max(prevY - 1, -34));
      }
      if (activeKeys.has("l")) {
        setRightY((prevY) => Math.min(prevY + 1, 40 * 3 + 9 - 34));
      }
    }, 16);

    addEventListener("keydown", handleKeyDown);
    addEventListener("keyup", handleKeyUp);

    return () => {
      clearInterval(intervalId);
      removeEventListener("keydown", handleKeyDown);
      removeEventListener("keyup", handleKeyUp);
    };
  }, [ballState.pause]);

  useEffect(() => {
    let t = setInterval(() => {
      if (ballState.pause) {
        return;
      }
      let x = ballState.x + ballState.vx;
      let y = ballState.y + ballState.vy;
      let vx = ballState.vx;
      let vy = ballState.vy;

      if (y < 21 || y > 200 - 21) {
        vy = vy * -1;
      }

      if (x < 18 && Math.abs(leftY + 35 + 34 - y) < 20) {
        vx = generateVelocity();
        vy = generateVelocity();
        x = 18;
      }
      if (x > 400 - 18 && Math.abs(rightY + 35 + 34 - y) < 20) {
        vx = generateVelocity();
        vy = generateVelocity();
        x = 400 - 18;
      }

      if (x > 400 - 5) {
        vx = vx * -1;
        if (y > 50 && y < 150) {
          setPunkty((punkty) => ({
            ...punkty,
            left: punkty.left + 1,
          }));
          setBallState(generateInitialBallState);
          setLeftY(30);
          setRightY(30);
          return;
        }
      }

      if (x < 5) {
        vx = vx * -1;
        if (y > 50 && y < 150) {
          setPunkty((punkty) => ({
            ...punkty,
            right: punkty.right + 1,
          }));
          setBallState(generateInitialBallState);
          setLeftY(30);
          setRightY(30);
          return;
        }
      }

      setBallState({ x: x, y: y, vx: vx, vy: vy });
    }, 16);
    return () => {
      clearInterval(t);
    };
  }, [ballState, punkty]);

  let onRestart = () => {
    setBallState(generateInitialBallState);
    setPunkty(resetujPunkty);
    setLeftY(30);
    setRightY(30);
  };

  let onPause = () => {
    let p = punkty.pauseTime;
    pauseStartedNew = punkty.pauseStarted;
    pauseTimeNew = punkty.pauseTime;
    if (!ballState.pause) {
      pauseStartedNew = Math.round(Date.now() / 1000);
    } else {
      pauseTimeNew += Math.round(Date.now() / 1000) - pauseStartedNew;
      pauseStarted = null;
    }
    setBallState({
      ...ballState,
      pause: !ballState.pause,
    });
    setPunkty({
      ...punkty,
      pauseStarted: pauseStartedNew,
      pauseTime: pauseTimeNew,
    });
  };

  return (
    <div className="App">
      <h2 className="text-border">PONG PGUI</h2>
      <div
        style={{
          display: "flex",
          flexDirection: "column",
          justifyContent: "center",
          position: "relative",
        }}
      >
        <Playground points={punkty} onRestart={onRestart} onPause={onPause} />
        <Paddle which="left" y={leftY} />
        <Paddle which="right" y={rightY} />
        <Ball ballState={ballState} />
      </div>
    </div>
  );
}
