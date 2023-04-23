using Core.Domain.Common;
using Core.Domain.Exceptions;

namespace Core.Domain.Entities;

public enum TagType
{
    NotDefined = 0,
    ForIncome = 1,
    ForOutcome = 2
}

public sealed class Tag : BaseEntity
{
    public Tag? Parent { get; private set; }

    private string name;

    public string Name
    {
        get => name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new BusinessException("Tag name must be provided");
            }
            
            if (value.Length > 20)
            {
                throw new BusinessException("Tag name is too long");
            }

            name = value;
        }
    }
    
    private TagType tagType;
    public TagType TagType
    {
        get => tagType;
        set
        {
            if (value == TagType.NotDefined)
            {
                throw new BusinessException("Tag type must be defined");
            }

            tagType = value;
        }
    }

    private readonly List<Transaction> transactions;
    public IReadOnlyCollection<Transaction> Transactions => transactions.AsReadOnly();
    
    private readonly List<Tag> children;
    public IReadOnlyCollection<Tag> Children => children.AsReadOnly();

    private Tag()
    {
        transactions = new List<Transaction>();
        children = new List<Tag>();
    }

    public static Tag Create(Tag? parent, string name, TagType tagType)
    {
        if (parent is not null)
        {
            if (tagType != TagType.ForOutcome)
            {
                throw new BusinessException("Only outcome type tags can belong to the parent");
            }
            
            if (parent.TagType != TagType.ForOutcome)
            {
                throw new BusinessException("Only outcome type tags can have any children");
            }
        }
        
        return new Tag
        {
            Parent = parent,
            Name = name,
            TagType = tagType,
        };
    }
}