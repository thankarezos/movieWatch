import { useState } from "react";
import "../../App.css";
import Background from "../Background/Background";
import Header from "../Header/Header";
import SearchResults from "../SearchResults/SearchResults";
import Results from "../Results/Results"

function Home() {
  const [isDone, setIsDone] = useState(false);
  return (
    <>
      <div>
        
        <Header />
        <div style={{display: "flex", justifyContent: "center", alignItems: "center", width: "99vw", height: "90vh"}}>
          {isDone? <Results /> : <SearchResults setIsDone={setIsDone}/>}
        </div>
      </div>
    </>
  );
}

export default Home;
