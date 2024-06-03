import { useEffect, useState } from "react";
import "../../App.css";
import Background from "../Background/Background";
import Header from "../Header/Header";
import SearchResults from "../SearchResults/SearchResults";
import Results from "../Results/Results";
import { Typography } from "antd";
import MovieCard from "../MovieCard/MovieCard";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faBookmark } from "@fortawesome/free-solid-svg-icons";
import { useNavigate } from "react-router-dom";

function Home() {
    const [hasChosen, setHasChosen] = useState(false);
    // if(reset){
    //     setHasChosen(false);
    // }

    // Sample data
    const movies = [
        { id: 1, title: "Inception", poster: "https://cdn11.bigcommerce.com/s-b72t4x/images/stencil/1280x1280/products/52630/56493/ST4568__76069.1624841046.jpg?c=2"},
        { id: 2, title: "Interstellar", poster: "https://cdn11.bigcommerce.com/s-b72t4x/images/stencil/1280x1280/products/52630/56493/ST4568__76069.1624841046.jpg?c=2"},
        { id: 3, title: "Interstellar", poster: "https://cdn11.bigcommerce.com/s-b72t4x/images/stencil/1280x1280/products/52630/56493/ST4568__76069.1624841046.jpg?c=2"},
        { id: 4, title: "Interstellar", poster: "https://cdn11.bigcommerce.com/s-b72t4x/images/stencil/1280x1280/products/52630/56493/ST4568__76069.1624841046.jpg?c=2"},
        { id: 5, title: "Interstellar", poster: "https://cdn11.bigcommerce.com/s-b72t4x/images/stencil/1280x1280/products/52630/56493/ST4568__76069.1624841046.jpg?c=2"},
        { id: 6, title: "Interstellar", poster: "https://cdn11.bigcommerce.com/s-b72t4x/images/stencil/1280x1280/products/52630/56493/ST4568__76069.1624841046.jpg?c=2"},
        { id: 7, title: "Interstellar", poster: "https://cdn11.bigcommerce.com/s-b72t4x/images/stencil/1280x1280/products/52630/56493/ST4568__76069.1624841046.jpg?c=2"},
        { id: 8, title: "Interstellar", poster: "https://cdn11.bigcommerce.com/s-b72t4x/images/stencil/1280x1280/products/52630/56493/ST4568__76069.1624841046.jpg?c=2"},
        { id: 9, title: "Interstellar", poster: "https://cdn11.bigcommerce.com/s-b72t4x/images/stencil/1280x1280/products/52630/56493/ST4568__76069.1624841046.jpg?c=2"},
        { id: 10, title: "Interstellar", poster: "https://cdn11.bigcommerce.com/s-b72t4x/images/stencil/1280x1280/products/52630/56493/ST4568__76069.1624841046.jpg?c=2"},
        { id: 11, title: "Interstellar", poster: "https://cdn11.bigcommerce.com/s-b72t4x/images/stencil/1280x1280/products/52630/56493/ST4568__76069.1624841046.jpg?c=2"},
        { id: 12, title: "Interstellar", poster: "https://cdn11.bigcommerce.com/s-b72t4x/images/stencil/1280x1280/products/52630/56493/ST4568__76069.1624841046.jpg?c=2"},
        { id: 13, title: "Interstellar", poster: "https://cdn11.bigcommerce.com/s-b72t4x/images/stencil/1280x1280/products/52630/56493/ST4568__76069.1624841046.jpg?c=2"},
        { id: 14, title: "Interstellar", poster: "https://cdn11.bigcommerce.com/s-b72t4x/images/stencil/1280x1280/products/52630/56493/ST4568__76069.1624841046.jpg?c=2"},
        { id: 15, title: "Interstellar", poster: "https://cdn11.bigcommerce.com/s-b72t4x/images/stencil/1280x1280/products/52630/56493/ST4568__76069.1624841046.jpg?c=2"},
    ];

    const [fav, setFav] = useState([1, 2, 3, 4, 7, 8, 9, 13, 14, 15]);

    
    
    const name = "Nick"
    
    const handleFavButton = (movie) => {
        setFav(prevFav => {
            if (prevFav.includes(movie.id)) {
                return prevFav.filter(id => id !== movie.id);
            } else {
                return [...prevFav, movie.id];
            }
        });
    }
  return (
    <>
      <Background />
      <Header />
      <div style={{display: "flex", justifyContent: "center", alignItems: "center", width: "99vw", height: "90vh"}}>
        {hasChosen ? <Results noRefresh={true} /> : <div
            style={{
            backgroundColor: "rgba(0, 0, 0, 0.9)",
            width: "1250px",
            height: "fit-content",
            minHeight: "488px",
            position: "absolute",
            left: "0px",
            top: "178px",
            transform: "translate(25%)",
            display: "flex",
            flexFlow: "row wrap",
            borderRadius: "10px",
            padding: "15px",
            boxShadow: "2px 2px 2px 2px black",
            }}
        >
            <div
            style={{
                display: "flex",
                flexDirection: "column",
                alignItems: "flex-start",
                width: "1200px",
                padding: "0px 10px",
            }}
            >
            <Typography.Title
                level={1}
                style={{
                color: "white",
                borderRadius: "20px",
                padding: "10px",
                margin: "0px auto 30px auto",
                textTransform: "capitalize",
                }}
            >
                Hello {name}!
            </Typography.Title>
            <Typography.Title level={3} style={{color: "white", padding: "10px", margin: "0px 0px 5px 0px"}}>
                Here are your favorite movies:
            </Typography.Title>
            </div>
            {movies.filter(movie => fav.includes(movie.id)).map((movie) => (
            <div
            key={movie.id}
            onClick={() => setHasChosen(true)}
                style={{
                position: "relative",
                display: "flex",
                justifyContent: "center",
                alignItems: "center",
                margin: "10px",
                cursor: "pointer"
                }}
            >
                <MovieCard title={movie.title} poster={movie.poster} />
                <FontAwesomeIcon icon={faBookmark} className="fav-btn" onClick={() => handleFavButton(movie)}/>
            </div>
            ))}
        </div>}
      </div>
    </>
  );
}

export default Home;
