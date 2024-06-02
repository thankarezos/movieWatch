import React, { useState, useEffect } from 'react';
import { Button, Input, Typography } from 'antd';
import { CheckCircleOutlined } from '@ant-design/icons'
import MovieCard from '../MovieCard/MovieCard';
import Background from '../Background/Background';

const SearchMovies = ({setIsDone}) => {
    // Sample data
    const initialMovies = [
        { id: 1, title: "Inception", poster: "https://cdn11.bigcommerce.com/s-b72t4x/images/stencil/1280x1280/products/52630/56493/ST4568__76069.1624841046.jpg?c=2", isChecked: false },
        { id: 2, title: "Interstellar", poster: "https://cdn11.bigcommerce.com/s-b72t4x/images/stencil/1280x1280/products/52630/56493/ST4568__76069.1624841046.jpg?c=2", isChecked: false },
        { id: 3, title: "Interstellar", poster: "https://cdn11.bigcommerce.com/s-b72t4x/images/stencil/1280x1280/products/52630/56493/ST4568__76069.1624841046.jpg?c=2", isChecked: false },
        { id: 4, title: "Interstellar", poster: "https://cdn11.bigcommerce.com/s-b72t4x/images/stencil/1280x1280/products/52630/56493/ST4568__76069.1624841046.jpg?c=2", isChecked: false },
        { id: 5, title: "Interstellar", poster: "https://cdn11.bigcommerce.com/s-b72t4x/images/stencil/1280x1280/products/52630/56493/ST4568__76069.1624841046.jpg?c=2", isChecked: false },
        { id: 6, title: "Interstellar", poster: "https://cdn11.bigcommerce.com/s-b72t4x/images/stencil/1280x1280/products/52630/56493/ST4568__76069.1624841046.jpg?c=2", isChecked: false },
        { id: 7, title: "Interstellar", poster: "https://cdn11.bigcommerce.com/s-b72t4x/images/stencil/1280x1280/products/52630/56493/ST4568__76069.1624841046.jpg?c=2", isChecked: false },
        { id: 8, title: "Interstellar", poster: "https://cdn11.bigcommerce.com/s-b72t4x/images/stencil/1280x1280/products/52630/56493/ST4568__76069.1624841046.jpg?c=2", isChecked: false },
        { id: 9, title: "Interstellar", poster: "https://cdn11.bigcommerce.com/s-b72t4x/images/stencil/1280x1280/products/52630/56493/ST4568__76069.1624841046.jpg?c=2", isChecked: false },
        { id: 10, title: "Interstellar", poster: "https://cdn11.bigcommerce.com/s-b72t4x/images/stencil/1280x1280/products/52630/56493/ST4568__76069.1624841046.jpg?c=2", isChecked: false },
        { id: 11, title: "Interstellar", poster: "https://cdn11.bigcommerce.com/s-b72t4x/images/stencil/1280x1280/products/52630/56493/ST4568__76069.1624841046.jpg?c=2", isChecked: false },
        { id: 12, title: "Interstellar", poster: "https://cdn11.bigcommerce.com/s-b72t4x/images/stencil/1280x1280/products/52630/56493/ST4568__76069.1624841046.jpg?c=2", isChecked: false },
        { id: 13, title: "Interstellar", poster: "https://cdn11.bigcommerce.com/s-b72t4x/images/stencil/1280x1280/products/52630/56493/ST4568__76069.1624841046.jpg?c=2", isChecked: false },
        { id: 14, title: "Interstellar", poster: "https://cdn11.bigcommerce.com/s-b72t4x/images/stencil/1280x1280/products/52630/56493/ST4568__76069.1624841046.jpg?c=2", isChecked: true },
        { id: 15, title: "Interstellar", poster: "https://cdn11.bigcommerce.com/s-b72t4x/images/stencil/1280x1280/products/52630/56493/ST4568__76069.1624841046.jpg?c=2", isChecked: true },
    ];

    
    const [movies, setMovies] = useState(initialMovies);
    const [searchTerm, setSearchTerm] = useState('');
    const [currentPage, setCurrentPage] = useState(1);
    const pageSize = 10;

    // Filtered and paginated movies
    const [filteredMovies, setFilteredMovies] = useState([]);
    const [currentMovies, setCurrentMovies] = useState([]);

    // Filtering movies based on search term
    useEffect(() => {
        const filtered = movies.filter(movie =>
            movie.title.toLowerCase().includes(searchTerm.toLowerCase())
        );
        setFilteredMovies(filtered);
    }, [searchTerm, movies]);

    // Updating pagination
    useEffect(() => {
        const newPageMovies = filteredMovies.slice((currentPage - 1) * pageSize, currentPage * pageSize);
        setCurrentMovies(newPageMovies);
    }, [currentPage, filteredMovies]);

    // Toggle movie check state
    const toggleMovieCheck = (id) => {
        const countChecked = movies.filter(movie => movie.isChecked).length;
        const movieToToggle = movies.find(movie => movie.id === id);
    
        if (!movieToToggle.isChecked && countChecked >= 5) {
            alert("You cannot check more than 5 movies!");
            return; // Prevent checking more than 5
        }
    
        const updatedMovies = movies.map(movie => {
            if (movie.id === id) {
                return { ...movie, isChecked: !movie.isChecked };
            }
            return movie;
        });
        setMovies(updatedMovies);
        // Ensure to filter and set current movies for display based on pagination
        const filtered = updatedMovies.filter(movie => 
            movie.title.toLowerCase().includes(searchTerm.toLowerCase())
        );
        setFilteredMovies(filtered);
        const newPageMovies = filtered.slice((currentPage - 1) * pageSize, currentPage * pageSize);
        setCurrentMovies(newPageMovies);
    };

    const handleSearch = (event) => {
        setSearchTerm(event.target.value);
        setCurrentPage(1);  // Ensures pagination reset on new search
    };

    const buttonHandler = () => {
        setIsDone(true);
    }

    return (
        <div style={{ marginTop: "100px",}}>
            <Background />
                <div style={{ backgroundColor: "rgba(0, 0, 0, 0.9)", width: "280px", height: "700px", alignContent: "start", zIndex: 10, position: "fixed", left: "0px", transform: "translate(50%)", top: "150px", display: "flex", flexFlow: "row wrap", borderRadius: "10px", padding: "15px", boxShadow: "2px 2px 2px 2px black", marginRight: "20px" }}>
                    {movies.filter(movie => movie.isChecked).map(movie => (
                        <div key={movie.id} onClick={() => toggleMovieCheck(movie.id)} style={{margin: "5px"}}>
                            <MovieCard title={movie.title} poster={movie.poster} size={100} />
                        </div>
                    ))}
                    <Button className='find-btn' style={{position: "absolute", bottom: "-50px", right: "0"}} onClick={buttonHandler}>Find Movies</Button>
                </div>
                <div style={{ backgroundColor: "rgba(0, 0, 0, 0.9)", width: "1250px", height: "fit-content", position: "relative", left: "0px", top: "178px", transform: "translate(14%)", display: "flex", flexFlow: "row wrap", borderRadius: "10px", padding: "15px", boxShadow: "2px 2px 2px 2px black"}}>
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
                            <div className={movie.isChecked? "checked" : ""}></div>
                            <CheckCircleOutlined className={movie.isChecked? "show-icon" : "hide-icon"}/>
                        </div>
                    ))}
                </div>
        </div>
    );
};

export default SearchMovies;