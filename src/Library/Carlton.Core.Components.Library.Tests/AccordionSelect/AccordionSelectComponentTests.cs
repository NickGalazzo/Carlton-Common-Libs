using AutoFixture;
using AutoFixture.Xunit2;
using Carlton.Core.Utilities.UnitTesting;

namespace Carlton.Core.Components.Library.Tests;

[Trait("Component", nameof(AccordionSelect<int>))]
public class AccordionSelectComponentTests : TestContext
{
    [Theory(DisplayName = "Markup Test")]
    [AutoData]
    [InlineAutoData(true)]
    [InlineAutoData(true)]
    [InlineAutoData(true, 0)]
    [InlineAutoData(true, 0)]
    public void AccordionSelect_Markup_RendersCorrectly(bool isExpanded, int numOfItems, Fixture fixture, string title)
    {
        //Arrange
        var items = fixture.CreateMany<SelectItem<int>>(numOfItems);

        //Act
        var cut = RenderComponent<AccordionSelect<int>>(parameters => parameters
            .Add(p => p.Title, title)
            .Add(p => p.IsExpanded, isExpanded)
            .Add(p => p.Items, items));

        //Assert
        cut.MarkupMatches(GenerateExpectedMarkup(title, isExpanded, items));
    }

    [Theory(DisplayName = "Title Parameter Test")]
    [AutoData]
    [InlineData("")]
    [InlineData(null)]
    public void AccordionSelect_TitleParam_RendersCorrectly(string title)
    {
        //Act
        var cut = RenderComponent<AccordionSelect<int>>(parameters => parameters
            .Add(p => p.Title, title));

        var titleElm = cut.Find(".item-group-name");

        //Assert
        Assert.Equal(title, titleElm.InnerHtml);
    }

    [Theory(DisplayName = "IsExpanded Parameter Test")]
    [InlineAutoData(false, "mdi-plus-box-outline")]
    [InlineAutoData(true, "mdi-minus-box-outline")]
    public void AccordionSelect_IsExpandedParam_RendersCorrectly(bool isExpanded, string expectedClass, string title)
    {
        //Act
        var cut = RenderComponent<AccordionSelect<int>>(parameters => parameters
            .Add(p => p.Title, title)
            .Add(p => p.IsExpanded, isExpanded));

        var itemContainerElement = cut.Find(".item-container");
        var accordionContainerElement = cut.Find(".accordion-icon-btn");
        var expectedAccordionValue = !isExpanded;

        //Assert
        Assert.Equal(expectedAccordionValue, itemContainerElement.ClassList.Contains("collapsed"));
        Assert.True(accordionContainerElement.ClassList.Contains(expectedClass));
    }

    [Theory(DisplayName = "Items Parameter Test"), AutoData]
    public void AccordionSelect_ItemsParams_RendersCorrectly(string title, ReadOnlyCollection<SelectItem<int>> items)
    {
        //Arrange
        var expectedCount = items.Count;
        var expectedItemNames = items.Select(_ => _.Name);

        //Act
        var cut = RenderComponent<AccordionSelect<int>>(parameters => parameters
                .Add(p => p.Title, title)
                .Add(p => p.IsExpanded, true)
                .Add(p => p.Items, items));

        var actualCount = cut.FindAll(".item").Count;
        var actualItemNames = cut.FindAll(".item-name").Select(_ => _.TextContent);


        //Assert
        Assert.Equal(expectedCount, actualCount);
        Assert.Equal(expectedItemNames, actualItemNames);
    }

    [Theory(DisplayName = "SelectedItemChanged Callback Parameter Test"), AutoData]
    public void AccordionSelect_SelectedItemChangedParam_FiresCallback(
        string title,
        IEnumerable<SelectItem<int>> items)
    {
        //Arrange
        var eventCalled = false;
        SelectItem<int>? selectedItem = null;
        var selectedIndex = TestingRndUtilities.GetRandomActiveIndex(items.Count());
        var expectedValue = items.ElementAt(selectedIndex).Value;

        var cut = RenderComponent<AccordionSelect<int>>(parameters => parameters
            .Add(p => p.Title, title)
            .Add(p => p.IsExpanded, true)
            .Add(p => p.Items, items)
            .Add(p => p.SelectedItemChanged, (selected) => { eventCalled = true; selectedItem = selected; }));

        var itemElms = cut.FindAll(".item");
        var selectedElement = itemElms[selectedIndex];

        //Act
        selectedElement.Click();

        //Assert
        Assert.True(eventCalled);
        Assert.NotNull(selectedItem);
        Assert.Equal(expectedValue, selectedItem.Value);
    }

    [Theory(DisplayName = "SelectedValue Parameter Test"), AutoData]
    public void AccordionSelect_SelectedValueParam_RendersCorrectly(string title, ReadOnlyCollection<SelectItem<int>> items)
    {
        //Arrange
        var selectedIndex = TestingRndUtilities.GetRandomActiveIndex(items.Count);
        var selectedValue = items.ElementAt(selectedIndex).Value;

        //Act
        var cut = RenderComponent<AccordionSelect<int>>(parameters => parameters
            .Add(p => p.Title, title)
            .Add(p => p.IsExpanded, true)
            .Add(p => p.Items, items)
            .Add(p => p.SelectedValue, selectedValue));

        var itemElms = cut.FindAll(".item");
        var selectedItem = itemElms[selectedIndex];

        //Assert
        Assert.True(selectedItem.ClassList.Contains("selected"));
    }

    [Theory(DisplayName = "SelectedValue Parameter, Not Provided Test"), AutoData]
    public void AccordionSelect_SelectedValueParamDoesNotExist_RendersCorrectly(
        string title,
        bool isExpanded,
        IEnumerable<SelectItem<int>> items)
    {
        //Act
        var cut = RenderComponent<AccordionSelect<int>>(parameters => parameters
            .Add(p => p.Title, title)
            .Add(p => p.IsExpanded, isExpanded)
            .Add(p => p.Items, items)
            .Add(p => p.SelectedValue, -1));

        var itemElms = cut.FindAll(".accordion-header");

        //Assert
        Assert.DoesNotContain("selected", itemElms.SelectMany(_ => _.ClassList));
    }

    [Theory(DisplayName = "Item Click Test"), AutoData]
    public void AccordionSelect_ItemClick_SetsValueCorrectly(
        string title,
        IEnumerable<SelectItem<int>> items)
    {
        //Arrange
        var selectedIndex = TestingRndUtilities.GetRandomActiveIndex(items.Count());
        var selectedValue = items.ElementAt(selectedIndex).Value;

        var cut = RenderComponent<AccordionSelect<int>>(parameters => parameters
            .Add(p => p.Title, title)
            .Add(p => p.IsExpanded, true)
            .Add(p => p.Items, items));

        var itemElms = cut.FindAll(".item");
        var selectedItem = itemElms[selectedIndex];

        //Act
        selectedItem.Click();

        //Assert
        Assert.Equal(selectedValue, cut.Instance.SelectedValue);
    }

    private static string GenerateExpectedMarkup(string title, bool isExpanded, IEnumerable<SelectItem<int>> items)
    {
        var iconType = isExpanded ? "minus" : "plus";
        var itemMarkup = string.Join(Environment.NewLine, items
            .Select(item => $@"
                <div class=""item"">
                    <span class=""icon mdi mdi-icon mdi-12px mdi-bookmark""></span>
                    <span class=""item-name"">{item.Name}</span>
                </div>
            "));

        return $@"
            <div class=""accordion-select"">
                <div class=""content"">
                    <div class=""accordion-header"">
                        <span class=""accordion-icon-btn mdi mdi-icon mdi-24px mdi-{iconType}-box-outline""></span>
                        <span class=""item-group-name"">{title}</span>
                    </div>
                    <div class=""item-container {(isExpanded ? string.Empty : "collapsed")}"">
                        {itemMarkup}
                    </div>
                </div>
            </div>
        ";
    }
}