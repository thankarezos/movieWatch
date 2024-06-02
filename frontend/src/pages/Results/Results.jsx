import { useState } from "react";
import "../../App.css";
import { Flex, Spin } from "antd"
import Background from "../Background/Background";
import MovieCard from "../MovieCard/MovieCard";

function Results() {
  const [isVisible, setIsVisible] = useState(true); // Make this true when the results are ready
  const refreshPick = () => {
    console.log("Refreshed!");
  }
  return (
      <div>
      <Background banner={"https://images-ext-1.discordapp.net/external/PZo3ITEUI-5yyGfuJp0wwkQ5vWOP8iBbeycfUCYKiEU/https/image.tmdb.org/t/p/original/8Qsr8pvDL3s1jNZQ4HK1d1Xlvnh.jpg"}/>
      <Spin size="large" style={{backgroundColor: "rgba(0,0,0,0.8)", padding: "15px", borderRadius: "50px", zIndex: "-100", position: "absolute", display: isVisible ? "none" : "block"}} />
      <Flex align="center" gap="middle" style={{position: "relative"}}>
        
        <div style={{ backgroundColor: "rgba(0, 0, 0, 0.9)", width: "1250px", height: "600px", display: isVisible ? "flex" : "none", flexFlow: "row wrap", borderRadius: "10px", padding: "15px", boxShadow: "2px 2px 2px 2px black"}}>
          <MovieCard refreshPick={refreshPick} isOverview={true} title={"Fantastic Beasts"} year={2024} rating={7.1} genre={["action", "comedy"]} poster={"https://image.tmdb.org/t/p/original/h6NYfVUyM6CDURtZSnBpz647Ldd.jpg"} desc={"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."}/>
        </div>
      </Flex>
      </div>
  );
}

export default Results;
