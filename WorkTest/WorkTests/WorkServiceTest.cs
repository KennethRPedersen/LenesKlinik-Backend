﻿using System;
using System.Collections.Generic;
using System.Linq;
using LenesKlinik.Core.ApplicationServices;
using LenesKlinik.Core.ApplicationServices.Implementation;
using LenesKlinik.Core.DomainServices;
using LenesKlinik.Core.Entities;
using Moq;
using Xunit;

namespace WorkTest
{
    public class WorkServiceTest
    {
        private readonly List<Work> _work;
        private readonly IWorkService _service;
        private readonly Mock<IWorkRepository> mock;

        // This constructor is called for each test, so that they may run simultaneously
        public WorkServiceTest()
        {
            _work = GetMockWork().ToList();
            mock = new Mock<IWorkRepository>();
            _service = new WorkService(mock.Object);
            Work w = _work[0];

            mock.Setup(repo => repo.GetWorkById(It.IsAny<int>()))
                .Returns<int>(id => _work.FirstOrDefault(work => work.Id == id));
            mock.Setup(repo => repo.UpdateWork(It.IsAny<Work>()))
                .Returns<Work>(x => x);
            mock.Setup(repo => repo.DeleteWork(It.IsAny<int>()));
            mock.Setup(repo => repo.GetAllWork())
                .Returns(GetMockWork);
            mock.Setup(repo => repo.CreateWork(It.IsAny<Work>()))
                .Returns(new Work
            {
                Id = 1,
                Title = w.Title,
                Description = w.Description,
                Duration = w.Duration,
                Price = w.Price,
                ImageUrl = "url.png"
            });

        }
        #region CREATE
        [Fact]
        public void CreateWorkSuccessTest()
        {
            Work w = _work[0];
            var returnWork = _service.CreateWork(w);
            mock.Verify(repo => repo.CreateWork(w), Times.Once);
            Assert.Equal(1, returnWork.Id);
        }

        [Fact]
        public void CreateWorkNoTitleExpectArgumentExceptionTest()
        {
            Work w = _work[0];
            w.Title = null;
            Exception e = Assert.Throws<ArgumentException>(() => _service.CreateWork(w));
            Assert.Equal("Title empty or null!", e.Message);
            mock.Verify(repo => repo.CreateWork(w), Times.Never);
        }

        [Fact]
        public void CreateWorkNoDescriptionExpectArgumentExceptionTest()
        {
            Work w = _work[0];
            w.Description = null;
            Exception e = Assert.Throws<ArgumentException>(() => _service.CreateWork(w));
            Assert.Equal("Description empty or null!", e.Message);
            mock.Verify(repo => repo.CreateWork(w), Times.Never);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void CreateWorkInvalidDurationExpectArgumentExceptionTest(int duration)
        {
            Work w = _work[0];
            w.Duration = duration;
            Exception e = Assert.Throws<ArgumentException>(() => _service.CreateWork(w));
            Assert.Equal("Duration cannot be 0 or less!", e.Message);
            mock.Verify(repo => repo.CreateWork(w), Times.Never);
        }

        [Theory]
        [InlineData(-1.00)]
        [InlineData(0.00)]
        public void CreateWorkInvalidPriceExpectArgumentExceptionTest(double price)
        {
            Work w = _work[0];
            w.Price = price;
            Exception e = Assert.Throws<ArgumentException>(() => _service.CreateWork(w));
            Assert.Equal("Price cannot be 0 or less!", e.Message);
            mock.Verify(repo => repo.CreateWork(w), Times.Never);
        }

        #endregion

        #region READ
        [Fact]
        public void GetAllWorkSuccessTest()
        {
            var returnWork = _service.GetAllWork();
            mock.Verify(repo => repo.GetAllWork(), Times.Once);
            Assert.Equal(2, returnWork.Count());
        }

        [Fact]
        public void GetWorkByIdSuccessTest()
        {
            Work w = _service.GetWorkById(1);
            Assert.Equal(1, w.Id);
            mock.Verify(repo => repo.GetWorkById(1), Times.Once);
        }

        [Fact]
        public void GetWorkByIdInvalidIdExpectArgumentExceptionTest()
        {
            Exception e = Assert.Throws<ArgumentException>(() => _service.GetWorkById(9999));
            Assert.Equal($"No entity found with id {9999}!", e.Message);
            mock.Verify(repo => repo.GetWorkById(9999), Times.Once);
        }

        #endregion

        #region DELETE
        [Fact]
        public void DeleteWorkSuccessTest()
        {
            _service.DeleteWork(1);
            mock.Verify(repo => repo.DeleteWork(1), Times.Once);
        }


        #endregion

        #region UPDATE

        [Fact]
        public void EditWorkSuccessTest()
        {
            Work w = _work[0];
            w.Title = "Edited title";

            Work returnWork = _service.UpdateWork(1, w);

            mock.Verify(repo => repo.UpdateWork(w), Times.Once);
            Assert.Equal("Edited title", w.Title);
        }

        [Fact]
        public void UpdateWorkIdMismatchExpectArgumentExceptionTest()
        {
            Work w = _work[0];

            Exception e = Assert.Throws<ArgumentException>(() => _service.UpdateWork(1337, w));

            Assert.Equal("Id mismatch", e.Message);
        }

        [Fact]
        public void UpdateWorkNoTitleExpectArgmentExceptionTest()
        {
            Work w = _work[0];
            w.Title = null;

            Exception e = Assert.Throws<ArgumentException>(() => _service.UpdateWork(1, w));

            Assert.Equal("Title empty or null!", e.Message);
        }

        [Fact]
        public void UpdateWorkNoTitleExpectArgumentExceptionTest()
        {
            Work w = _work[0];
            w.Title = null;
            Exception e = Assert.Throws<ArgumentException>(() => _service.UpdateWork(1, w));
            Assert.Equal("Title empty or null!", e.Message);
            mock.Verify(repo => repo.CreateWork(w), Times.Never);
        }

        [Fact]
        public void UpdateWorkNoDescriptionExpectArgumentExceptionTest()
        {
            Work w = _work[0];
            w.Description = null;
            Exception e = Assert.Throws<ArgumentException>(() => _service.UpdateWork(1, w));
            Assert.Equal("Description empty or null!", e.Message);
            mock.Verify(repo => repo.CreateWork(w), Times.Never);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void UpdateWorkInvalidDurationExpectArgumentExceptionTest(int duration)
        {
            Work w = _work[0];
            w.Duration = duration;
            Exception e = Assert.Throws<ArgumentException>(() => _service.UpdateWork(1, w));
            Assert.Equal("Duration cannot be 0 or less!", e.Message);
            mock.Verify(repo => repo.CreateWork(w), Times.Never);
        }

        [Theory]
        [InlineData(-1.00)]
        [InlineData(0.00)]
        public void UpdateWorkInvalidPriceExpectArgumentExceptionTest(double price)
        {
            Work w = _work[0];
            w.Price = price;
            Exception e = Assert.Throws<ArgumentException>(() => _service.UpdateWork(1, w));
            Assert.Equal("Price cannot be 0 or less!", e.Message);
            mock.Verify(repo => repo.CreateWork(w), Times.Never);
        }

        #endregion

        private IEnumerable<Work> GetMockWork()
        {
            return new List<Work>()
            {
                new Work
                {
                    Id = 1,
                    Title = "Massage",
                    Description = "A nice massage",
                    Duration = 30,
                    Price = 299.99,
                    ImageUrl = "Image.png"
                },
                new Work
                {
                    Id = 2,
                    Title = "Raindrop Massage",
                    Description = "A nice massage",
                    Duration = 30,
                    Price = 299.99,
                    ImageUrl = "Image.png"
                },
            };
        }

    }

}