// import { useState } from 'react';
import "../App.css";
import bgImage from "../assets/collage5.jpg";

function Background() {
  return (
    <>
        <div className="bg-container">
          <img src={bgImage} alt="" className="bg-image" />
        </div>
    </>
  );
}

export default Background;
