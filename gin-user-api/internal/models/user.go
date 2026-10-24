package models

import (
	"time"
)

// User represents the persistent database entity.
type User struct {
	UserId    uint       `gorm:"primaryKey;autoIncrement;column:user_id" json:"userId"`
	FirstName string     `gorm:"size:50;not null;column:first_name" json:"firstName"`
	LastName  string     `gorm:"size:50;not null;column:last_name" json:"lastName"`
	Email     string     `gorm:"size:100;uniqueIndex;not null;column:email" json:"email"`
	CreatedAt time.Time  `gorm:"not null;column:created_at" json:"createdAt"`
	UpdatedAt *time.Time `gorm:"column:updated_at" json:"updatedAt"`
}

// CreateUserRequest validates payload for creating a user.
type CreateUserRequest struct {
	FirstName string `json:"firstName" binding:"required,min=1,max=50"`
	LastName  string `json:"lastName" binding:"required,min=1,max=50"`
	Email     string `json:"email" binding:"required,email,max=100"`
}

// UpdateUserRequest validates payload for updating an existing user.
type UpdateUserRequest struct {
	FirstName string `json:"firstName" binding:"required,min=1,max=50"`
	LastName  string `json:"lastName" binding:"required,min=1,max=50"`
	Email     string `json:"email" binding:"required,email,max=100"`
}

// ApiErrorResponse represents a standardized JSON error response.
type ApiErrorResponse struct {
	StatusCode int       `json:"statusCode"`
	Message    string    `json:"message"`
	Details    []string  `json:"details,omitempty"`
	Timestamp  time.Time `json:"timestamp"`
}
