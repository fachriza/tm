package repository

import (
	"errors"
	"strings"
	"time"

	"gin-user-api/internal/models"
	"github.com/glebarez/sqlite"
	"gorm.io/gorm"
	"gorm.io/gorm/logger"
)

type UserRepository interface {
	GetAll() ([]models.User, error)
	GetByID(id uint) (*models.User, error)
	GetByEmail(email string) (*models.User, error)
	Create(req *models.CreateUserRequest) (*models.User, error)
	Update(id uint, req *models.UpdateUserRequest) (*models.User, error)
	Delete(id uint) error
}

type sqlUserRepository struct {
	db *gorm.DB
}

// InitDatabase initializes the SQLite database and performs auto-migration.
func InitDatabase(dbPath string) (*gorm.DB, error) {
	db, err := gorm.Open(sqlite.Open(dbPath), &gorm.Config{
		Logger: logger.Default.LogMode(logger.Silent),
	})
	if err != nil {
		return nil, err
	}

	// Auto-migrate schema
	if err := db.AutoMigrate(&models.User{}); err != nil {
		return nil, err
	}

	return db, nil
}

func NewUserRepository(db *gorm.DB) UserRepository {
	return &sqlUserRepository{db: db}
}

func (r *sqlUserRepository) GetAll() ([]models.User, error) {
	var users []models.User
	if err := r.db.Order("user_id ASC").Find(&users).Error; err != nil {
		return nil, err
	}
	return users, nil
}

func (r *sqlUserRepository) GetByID(id uint) (*models.User, error) {
	var user models.User
	if err := r.db.First(&user, id).Error; err != nil {
		if errors.Is(err, gorm.ErrRecordNotFound) {
			return nil, nil
		}
		return nil, err
	}
	return &user, nil
}

func (r *sqlUserRepository) GetByEmail(email string) (*models.User, error) {
	var user models.User
	normalized := strings.ToLower(strings.TrimSpace(email))
	if err := r.db.Where("LOWER(email) = ?", normalized).First(&user).Error; err != nil {
		if errors.Is(err, gorm.ErrRecordNotFound) {
			return nil, nil
		}
		return nil, err
	}
	return &user, nil
}

func (r *sqlUserRepository) Create(req *models.CreateUserRequest) (*models.User, error) {
	now := time.Now().UTC()
	user := models.User{
		FirstName: strings.TrimSpace(req.FirstName),
		LastName:  strings.TrimSpace(req.LastName),
		Email:     strings.ToLower(strings.TrimSpace(req.Email)),
		CreatedAt: now,
	}

	if err := r.db.Create(&user).Error; err != nil {
		return nil, err
	}
	return &user, nil
}

func (r *sqlUserRepository) Update(id uint, req *models.UpdateUserRequest) (*models.User, error) {
	var user models.User
	if err := r.db.First(&user, id).Error; err != nil {
		if errors.Is(err, gorm.ErrRecordNotFound) {
			return nil, nil
		}
		return nil, err
	}

	now := time.Now().UTC()
	user.FirstName = strings.TrimSpace(req.FirstName)
	user.LastName = strings.TrimSpace(req.LastName)
	user.Email = strings.ToLower(strings.TrimSpace(req.Email))
	user.UpdatedAt = &now

	if err := r.db.Save(&user).Error; err != nil {
		return nil, err
	}
	return &user, nil
}

func (r *sqlUserRepository) Delete(id uint) error {
	result := r.db.Delete(&models.User{}, id)
	if result.Error != nil {
		return result.Error
	}
	if result.RowsAffected == 0 {
		return gorm.ErrRecordNotFound
	}
	return nil
}
