const express = require("express");
const mysql = require("mysql");
const fs = require('fs');
const app = express();

app.use(express.json());
const port = process.env.PORT || 8080;
app.listen(port, ()=> {
    console.log(`REST API listening on port ${port}`);
});

app.get("/", async(req, res) => {
    res.json({ status: "up and running"});
});

app.get("/:department", async(req, res) => {
    const query = "SELECT * FROM department";
    pool.query(query, [], (error, results) => {
        if(!results[0]){
            res.json({status: "Not Found"});
        }else{
            res.json(results[0]);
        }
    });
});

const pool = mysql.createPool(
    {
        host:"capstonedb01.mysql.database.azure.com", 
        user:"capstoneadmin", 
        password:"DBadmin01!", 
        database:"classyschedule", 
        //port:3306, 
        //ssl:{ca:fs.readFileSync("{ca-cert filename}")}
    });
