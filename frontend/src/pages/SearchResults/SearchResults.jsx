import { useState, useEffect } from 'react';
import { Input, Typography } from 'antd';
import { CheckCircleOutlined } from '@ant-design/icons'
import MovieCard from '../MovieCard/MovieCard';
import Background from '../Background/Background';
import apiService from '../../ApiService';
import PropTypes from "prop-types";
import Left from '../Left/Left';

const SearchMovies = ({setIsDone, setCheckedMovies}) => {

    const [movies, setMovies] = useState([]);
    const [checkedMoviesArray, setCheckedMoviesArray] = useState([]);

    useEffect(() => {
        // Fetch data immediately when component mounts
        fetchMovies();
      }, []);

    const fetchMovies = async () => {
    const response = await apiService.get("/Movies?Page=1&PageSize=15");
        const movies = response.data.data.movies;
        const newMovies = movies.map((movie) => {
            return {
                id: movie.id,
                title: movie.title,
                poster: movie.imageUrl
            };
        });
        setMovies(newMovies);
    };

    const fetchMoviesWithString = async (searchTerm) => {
        const response = await apiService.get("/Movies?Page=1&PageSize=15&TitleFilter=" + searchTerm);
            const movies = response.data.data.movies;
            const newMovies = movies.map((movie) => {
                return {
                    id: movie.id,
                    title: movie.title,
                    poster: movie.imageUrl
                };
            });
            setMovies(newMovies);
    };

    // Sample data
    

    const [searchTerm, setSearchTerm] = useState('');
    // const [currentPage, setCurrentPage] = useState(1);
    // const pageSize = 10;

    // Filtered and paginated movies
    // const [filteredMovies, setFilteredMovies] = useState([]);
    // const [currentMovies, setCurrentMovies] = useState([]);

    // Filtering movies based on search term
    // useEffect(() => {
    //     const filtered = movies.filter(movie =>
    //         movie.title.toLowerCase().includes(searchTerm.toLowerCase())
    //     );
    //     setFilteredMovies(filtered);
    // }, [searchTerm, movies]);

    // Updating pagination
    // useEffect(() => {
    //     const newPageMovies = filteredMovies.slice((currentPage - 1) * pageSize, currentPage * pageSize);
    //     setCurrentMovies(newPageMovies);
    // }, [currentPage, filteredMovies]);

    // Toggle movie check state
    const toggleMovieCheck = (id) => {
        console.log("Toggle Movie Check: ", id);
        const countChecked = checkedMoviesArray.length;
        const movieToToggle = movies.find(movie => movie.id === id);
        console.log("Movie to Toggle: ", movieToToggle);

        if (!movieToToggle) {
            console.error("Movie not found");
            return;
        }

        const isMovieChecked = checkedMoviesArray.some(movie => movie.id === id);

        let updatedMovies;

        if (isMovieChecked) {
            // Remove the movie if it is already checked
            updatedMovies = checkedMoviesArray.filter(movie => movie.id !== id);
        } else {
            // Check if the count is less than 5 before adding
            if (countChecked >= 5) {
                alert("You cannot check more than 5 movies!");
                return; // Prevent checking more than 5
            }
            // Add the entire movie object to the checked movies array
            updatedMovies = [...checkedMoviesArray, movieToToggle];
        }

        setCheckedMoviesArray(updatedMovies);
    };

    const handleSearch = (event) => {
        setSearchTerm(event.target.value);
        // setCurrentPage(1);  // Ensures pagination reset on new search
        fetchMoviesWithString(event.target.value);
    };

    const buttonHandler = () => {
        const countChecked = checkedMoviesArray.length;
        if (countChecked < 5) {
            alert("You need to choose 5 movies!");
            return; // Prevent checking more than 5
        }
        console.log("Find Movies");
        setIsDone();
        console.log("Checked Movies: ", movies.filter(movie => movie.isChecked));

        const ids = checkedMoviesArray.map(movie => movie.id);

        setCheckedMovies(ids);
    }

    return (
        <div style={{ marginTop: "200px",}}>
            <Background />
                <Left checkedMoviesArray={checkedMoviesArray} toggleMovieCheck={toggleMovieCheck} buttonHandler={buttonHandler}/>
                <div style={{ backgroundColor: "rgba(0, 0, 0, 0.9)", width: "1050px", height: "fit-content", position: "relative", left: "0px", top: "178px", transform: "translate(24%)", display: "flex", flexFlow: "row wrap", borderRadius: "10px", padding: "15px", boxShadow: "2px 2px 2px 2px black"}}>
                    <div style={{display: "flex", flexDirection: "row", justifyContent: "space-between", alignItems: "center", width: "1200px", padding: "0px 10px"}}>
                        <Typography.Title level={3} style={{color: "white", backgroundColor: "rgba(0, 0, 0, 0.2)", borderRadius: "20px", padding: "8px", margin: "0px 0px 30px 0px"}}>
                            Choose 5 movies you like:
                        </Typography.Title>
                        <Input
                            placeholder="Search movies"
                            value={searchTerm}
                            onChange={handleSearch}
                            style={{ marginBottom: 20, marginRight: "15px", width: 300, borderRadius: "20px" }}
                        />
                    </div>
                    {movies.map(movie => (
                        <div key={movie.id} onClick={() => toggleMovieCheck(movie.id)} style={{position: "relative", display: "flex", justifyContent: "center", alignItems: "center", cursor: "pointer", margin: "10px"}}>
                            <MovieCard title={movie.title} poster={movie.poster} />
                            <div className={checkedMoviesArray.find(m => m.id === movie.id)? "checked" : ""}></div>
                            <CheckCircleOutlined className={checkedMoviesArray.find(m => m.id === movie.id)? "show-icon" : "hide-icon"}/>
                        </div>
                    ))}
                </div>
        </div>
    );
};

SearchMovies.propTypes = {
    setIsDone: PropTypes.func.isRequired,
    setCheckedMovies: PropTypes.func.isRequired
  };

export default SearchMovies;