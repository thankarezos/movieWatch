import { useEffect, useState } from "react";
import "../../App.css";
import Background from "../Background/Background";
import Header from "../Header/Header";
import Results from "../Results/Results";
import { Typography } from "antd";
import MovieCard from "../MovieCard/MovieCard";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faBookmark } from "@fortawesome/free-solid-svg-icons";
import apiService from "../../ApiService";

function Home() {
    const [hasChosen, setHasChosen] = useState(false);
    // if(reset){
    //     setHasChosen(false);
    // }
    const [activeMovie, setActiveMovie] = useState(null);
    const [movies, setMovies] = useState([]);
    const [name, setName] = useState("User");

    useEffect(() => {
        // Fetch data immediately when component mounts
        fetchProfile();
      }, []);

    const fetchProfile = async () => {
    const response = await apiService.get("/User/profile");
        const movies = response.data.data.favorites;
        const newMovies = movies.map((movie) => {
            return {
                id: movie.id,
                title: movie.title,
                poster: movie.imageUrl,
                banner: movie.bannerUrl,
                desc: movie.description,
                year: movie.year,
                rating: movie.rating,
                genres: movie.genres,
            };
        });
        setMovies(newMovies);
        setName(response.data.data.username);
    };

    // Sample data

    const removeMovie = async (id) => {
        const payload = {
            movieIds: [
                id
            ]
        }
        await apiService.post("/Movies/RemoveFavorite", payload);
        fetchProfile();
    }
    
    const handleFavButton = (movie) => {
        console.log(movie);
        removeMovie(movie.id);
    }

    const handleMovieClick = (movie) => {
        
        setHasChosen(true);
        setActiveMovie(movie);
    }
  return (
    <>
      <Background />
      <Header />
      <div style={{display: "flex", justifyContent: "center", alignItems: "center", width: "99vw", height: "90vh"}}>
        {hasChosen ? <Results noRefresh={true}  
                        id={activeMovie.id}
                        banner={activeMovie.banner} 
                        poster={activeMovie.poster}
                        desc={activeMovie.desc}
                        year={activeMovie.year} 
                        rating={activeMovie.rating} 
                        genre={activeMovie.genres}/> : <div
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
            {movies.map((movie) => (
            <div
            key={movie.id}
            onClick={() => handleMovieClick(movie)}
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
                <FontAwesomeIcon icon={faBookmark} className="fav-btn" onClick={(e) => {handleFavButton(movie); e.stopPropagation();}} style={{zIndex: 10000}}/>
            </div>
            ))}
        </div>}
      </div>
    </>
  );
}

export default Home;
