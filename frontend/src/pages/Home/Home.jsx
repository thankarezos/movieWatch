import { useState } from "react";
import "../../App.css";
import Header from "../Header/Header";
import SearchResults from "../SearchResults/SearchResults";
import Results from "../Results/Results"
import apiService from "../../ApiService";

function Home() {
  const [isDone, setIsDone] = useState(false);
  const [recommendations, setRecommendations] = useState([]);

  const [activeMovie, setActiveMovie] = useState({});
  const [activeIndex, setActiveIndex] = useState(0);

  const getRecommendations = async (ids) => {

    const checkedMoviesString = ids.join(",");

    console.log(checkedMoviesString);

    const response = await apiService.get("/Movies/RecoMultiple?ids=" + checkedMoviesString);

    const recommendations = response.data.data;

    console.log(recommendations);

    const newRecommendations = recommendations.map((movie) => {
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

    setRecommendations(newRecommendations);

    setActiveMovie(newRecommendations[0]);

    setIsDone(true);

    console.log(recommendations);
  }


  const setCheckedMovieHandler = (movies) => {
    console.log(movies);
    getRecommendations(movies);
  }


  const refreshPick = () => {
    const newActiveIndex = activeIndex + 1;
    const newActiveMovie = recommendations[newActiveIndex];
    setActiveIndex(newActiveIndex);
    setActiveMovie(newActiveMovie);
  }

 
  return (
    <>
      <div>
        
        <Header />
        <div style={{display: "flex", justifyContent: "center", alignItems: "center", width: "99vw", height: "90vh"}}>
          {isDone? <Results
                        refreshPick={refreshPick}
                        title={activeMovie.title}
                        id={activeMovie.id}
                        banner={activeMovie.banner} 
                        poster={activeMovie.poster}
                        desc={activeMovie.desc}
                        year={activeMovie.year} 
                        rating={activeMovie.rating} 
                        genre={activeMovie.genres}/>
              
          : <SearchResults setIsDone={setIsDone} setCheckedMovies={setCheckedMovieHandler}/>}
        </div>
      </div>
    </>
  );
}

export default Home;
