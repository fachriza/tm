package main

import (
	"log"
	"os"

	"gin-user-api/internal/handlers"
	"gin-user-api/internal/repository"
	"github.com/gin-gonic/gin"
)

func main() {
	// Set Gin mode
	mode := os.Getenv("GIN_MODE")
	if mode == "" {
		gin.SetMode(gin.ReleaseMode)
	}

	// Initialize SQLite Database
	dbPath := os.Getenv("DB_PATH")
	if dbPath == "" {
		dbPath = "users_gin.db"
	}

	db, err := repository.InitDatabase(dbPath)
	if err != nil {
		log.Fatalf("Failed to initialize database: %v", err)
	}

	repo := repository.NewUserRepository(db)
	userHandler := handlers.NewUserHandler(repo)

	// Create Gin Router
	router := gin.New()
	router.Use(gin.Logger(), gin.Recovery())

	// Health check endpoint
	router.GET("/health", func(c *gin.Context) {
		c.JSON(200, gin.H{"status": "healthy", "service": "gin-user-api"})
	})

	// User CRUD REST Endpoints
	api := router.Group("/api/users")
	{
		api.GET("", userHandler.GetAllUsers)
		api.POST("", userHandler.CreateUser)
		api.GET("/:userId", userHandler.GetUserByID)
		api.PUT("/:userId", userHandler.UpdateUser)
		api.DELETE("/:userId", userHandler.DeleteUser)
	}

	port := os.Getenv("PORT")
	if port == "" {
		port = "8080"
	}

	log.Printf("Starting Gin User API on http://localhost:%s", port)
	if err := router.Run(":" + port); err != nil {
		log.Fatalf("Failed to start server: %v", err)
	}
}
