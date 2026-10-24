package handlers

import (
	"errors"
	"fmt"
	"net/http"
	"strconv"
	"strings"
	"time"

	"gin-user-api/internal/models"
	"gin-user-api/internal/repository"
	"github.com/gin-gonic/gin"
	"github.com/go-playground/validator/v10"
	"gorm.io/gorm"
)

type UserHandler struct {
	repo repository.UserRepository
}

func NewUserHandler(repo repository.UserRepository) *UserHandler {
	return &UserHandler{repo: repo}
}

// GetAllUsers handles GET /api/users
func (h *UserHandler) GetAllUsers(c *gin.Context) {
	users, err := h.repo.GetAll()
	if err != nil {
		c.JSON(http.StatusInternalServerError, models.ApiErrorResponse{
			StatusCode: http.StatusInternalServerError,
			Message:    "Failed to retrieve users",
			Timestamp:  time.Now().UTC(),
		})
		return
	}
	if users == nil {
		users = []models.User{}
	}
	c.JSON(http.StatusOK, users)
}

// GetUserByID handles GET /api/users/:userId
func (h *UserHandler) GetUserByID(c *gin.Context) {
	id, err := strconv.ParseUint(c.Param("userId"), 10, 32)
	if err != nil || id == 0 {
		c.JSON(http.StatusBadRequest, models.ApiErrorResponse{
			StatusCode: http.StatusBadRequest,
			Message:    "UserId must be a positive integer.",
			Timestamp:  time.Now().UTC(),
		})
		return
	}

	user, err := h.repo.GetByID(uint(id))
	if err != nil {
		c.JSON(http.StatusInternalServerError, models.ApiErrorResponse{
			StatusCode: http.StatusInternalServerError,
			Message:    "Failed to retrieve user",
			Timestamp:  time.Now().UTC(),
		})
		return
	}

	if user == nil {
		c.JSON(http.StatusNotFound, models.ApiErrorResponse{
			StatusCode: http.StatusNotFound,
			Message:    fmt.Sprintf("User with ID %d not found.", id),
			Timestamp:  time.Now().UTC(),
		})
		return
	}

	c.JSON(http.StatusOK, user)
}

// CreateUser handles POST /api/users
func (h *UserHandler) CreateUser(c *gin.Context) {
	var req models.CreateUserRequest
	if err := c.ShouldBindJSON(&req); err != nil {
		details := formatValidationErrors(err)
		c.JSON(http.StatusBadRequest, models.ApiErrorResponse{
			StatusCode: http.StatusBadRequest,
			Message:    "Validation error",
			Details:    details,
			Timestamp:  time.Now().UTC(),
		})
		return
	}

	// Check email uniqueness
	existing, err := h.repo.GetByEmail(req.Email)
	if err != nil {
		c.JSON(http.StatusInternalServerError, models.ApiErrorResponse{
			StatusCode: http.StatusInternalServerError,
			Message:    "Database error during validation",
			Timestamp:  time.Now().UTC(),
		})
		return
	}
	if existing != nil {
		c.JSON(http.StatusConflict, models.ApiErrorResponse{
			StatusCode: http.StatusConflict,
			Message:    fmt.Sprintf("User with email '%s' already exists.", req.Email),
			Timestamp:  time.Now().UTC(),
		})
		return
	}

	user, err := h.repo.Create(&req)
	if err != nil {
		c.JSON(http.StatusInternalServerError, models.ApiErrorResponse{
			StatusCode: http.StatusInternalServerError,
			Message:    "Failed to create user",
			Timestamp:  time.Now().UTC(),
		})
		return
	}

	location := fmt.Sprintf("/api/users/%d", user.UserId)
	c.Header("Location", location)
	c.JSON(http.StatusCreated, user)
}

// UpdateUser handles PUT /api/users/:userId
func (h *UserHandler) UpdateUser(c *gin.Context) {
	id, err := strconv.ParseUint(c.Param("userId"), 10, 32)
	if err != nil || id == 0 {
		c.JSON(http.StatusBadRequest, models.ApiErrorResponse{
			StatusCode: http.StatusBadRequest,
			Message:    "UserId must be a positive integer.",
			Timestamp:  time.Now().UTC(),
		})
		return
	}

	var req models.UpdateUserRequest
	if err := c.ShouldBindJSON(&req); err != nil {
		details := formatValidationErrors(err)
		c.JSON(http.StatusBadRequest, models.ApiErrorResponse{
			StatusCode: http.StatusBadRequest,
			Message:    "Validation error",
			Details:    details,
			Timestamp:  time.Now().UTC(),
		})
		return
	}

	// Verify user exists
	existingUser, err := h.repo.GetByID(uint(id))
	if err != nil {
		c.JSON(http.StatusInternalServerError, models.ApiErrorResponse{
			StatusCode: http.StatusInternalServerError,
			Message:    "Database error",
			Timestamp:  time.Now().UTC(),
		})
		return
	}
	if existingUser == nil {
		c.JSON(http.StatusNotFound, models.ApiErrorResponse{
			StatusCode: http.StatusNotFound,
			Message:    fmt.Sprintf("User with ID %d not found.", id),
			Timestamp:  time.Now().UTC(),
		})
		return
	}

	// Verify email not taken by another user
	emailOwner, err := h.repo.GetByEmail(req.Email)
	if err != nil {
		c.JSON(http.StatusInternalServerError, models.ApiErrorResponse{
			StatusCode: http.StatusInternalServerError,
			Message:    "Database error",
			Timestamp:  time.Now().UTC(),
		})
		return
	}
	if emailOwner != nil && emailOwner.UserId != uint(id) {
		c.JSON(http.StatusConflict, models.ApiErrorResponse{
			StatusCode: http.StatusConflict,
			Message:    fmt.Sprintf("Email '%s' is already in use by another user.", req.Email),
			Timestamp:  time.Now().UTC(),
		})
		return
	}

	updatedUser, err := h.repo.Update(uint(id), &req)
	if err != nil {
		c.JSON(http.StatusInternalServerError, models.ApiErrorResponse{
			StatusCode: http.StatusInternalServerError,
			Message:    "Failed to update user",
			Timestamp:  time.Now().UTC(),
		})
		return
	}

	c.JSON(http.StatusOK, updatedUser)
}

// DeleteUser handles DELETE /api/users/:userId
func (h *UserHandler) DeleteUser(c *gin.Context) {
	id, err := strconv.ParseUint(c.Param("userId"), 10, 32)
	if err != nil || id == 0 {
		c.JSON(http.StatusBadRequest, models.ApiErrorResponse{
			StatusCode: http.StatusBadRequest,
			Message:    "UserId must be a positive integer.",
			Timestamp:  time.Now().UTC(),
		})
		return
	}

	err = h.repo.Delete(uint(id))
	if err != nil {
		if errors.Is(err, gorm.ErrRecordNotFound) {
			c.JSON(http.StatusNotFound, models.ApiErrorResponse{
				StatusCode: http.StatusNotFound,
				Message:    fmt.Sprintf("User with ID %d not found.", id),
				Timestamp:  time.Now().UTC(),
			})
			return
		}
		c.JSON(http.StatusInternalServerError, models.ApiErrorResponse{
			StatusCode: http.StatusInternalServerError,
			Message:    "Failed to delete user",
			Timestamp:  time.Now().UTC(),
		})
		return
	}

	c.Status(http.StatusNoContent)
}

func formatValidationErrors(err error) []string {
	var errMsgs []string
	var valErrs validator.ValidationErrors
	if errors.As(err, &valErrs) {
		for _, f := range valErrs {
			switch f.Tag() {
			case "required":
				errMsgs = append(errMsgs, fmt.Sprintf("%s is required.", f.Field()))
			case "email":
				errMsgs = append(errMsgs, fmt.Sprintf("%s must be a valid email address.", f.Field()))
			case "min":
				errMsgs = append(errMsgs, fmt.Sprintf("%s must be at least %s characters long.", f.Field(), f.Param()))
			case "max":
				errMsgs = append(errMsgs, fmt.Sprintf("%s cannot exceed %s characters.", f.Field(), f.Param()))
			default:
				errMsgs = append(errMsgs, fmt.Sprintf("%s failed validation tag '%s'.", f.Field(), f.Tag()))
			}
		}
	} else {
		errMsgs = append(errMsgs, strings.TrimSpace(err.Error()))
	}
	return errMsgs
}
